using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Services.Services;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shared.SystemEnums;

namespace Services
{
    public interface IMeetingTaskService : IBaseService
    {
        Result Get(Guid id);
        Result GetAll(DataSource dataSource);
        Result Create(MeetingTaskDto meetingTask);
        Result Update(MeetingTaskDto meetingTask);
        Result Delete(Guid id);
        Result CloseTask(Guid id);
    }
    
    public class MeetingTaskService : BaseService, IMeetingTaskService
    {

        public MeetingTaskService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }

        //public Result ChangeMeetingTaskStatus(Guid id, int status)
        //{
        //    var Task = _Context.MeetingTask.Where(x => x.Id == id).FirstOrDefault();
        //    if (Task == null)
        //        return new Error(ErrorMsg.NotFound.ToDescriptionString());
        //    Task.Status = status;
        //    _Context.SaveChanges();
        //    return new Success(_mapper.Map<MeetingTaskDto>(Task));
        //}
        public Result CloseTask(Guid id)
        {
            var Task = _Context.MeetingTask.Where(x => x.Id == id).FirstOrDefault();
            if (Task == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            if (Task.AssigneeId != CurrentUser.Id)
                return new Error(ErrorMsg.NotTaskAssignee.ToDescriptionString());
            Task.Status = 1;
            _Context.SaveChanges();
            TaskyIntegrationService Tasky = new TaskyIntegrationService(CurrentUser, _appSettings.TaskyHost);
            string Status = Tasky.MarkTaskAsCompleted(Task.RelatedTaskId.ToString());
            return new Success(_mapper.Map<MeetingTaskDto>(Task), ResponseMessage.TaskClosed.ToDescriptionString());
        }
        public Result Create(MeetingTaskDto meetingTask)
        {
            var xu = CurrentUser;
            var meeting = _Context.Meeting.Where(x => x.Id == meetingTask.MeetingId).FirstOrDefault();

            if(meeting==null)
                return new Error(ErrorMsg.NotValid.ToDescriptionString());

            if (!IsValidTask(meetingTask.DueDate, meeting.From))
            {
                return new Error(ErrorMsg.TaskDueDateInPast.ToDescriptionString());
            }
            if (!IsValidTask(meetingTask.AssigneeId,meetingTask.MeetingId))
            {
                return new Error(ErrorMsg.TaskAssigneeNotMeetingParticipant.ToDescriptionString());
            }
            else if(!IsUniqueTask(meetingTask))
            {
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
            }
            else
            {
                var Task = _mapper.Map<MeetingTask>(meetingTask);

                TaskyIntegrationService Tasky = new TaskyIntegrationService(CurrentUser, _appSettings.TaskyHost);

                string TaskID = Tasky.CreateTask(Task);


             if (TaskID != null)   Task.RelatedTaskId = Guid.Parse(TaskID);
                _Context.MeetingTask.Add(Task);
                _Context.SaveChanges();
                meetingTask.Id = Task.Id;
                return new Success(meetingTask, ResponseMessage.TaskCreated.ToDescriptionString());
            }
        }

        public Result Delete(Guid id)
        {
            var Task = _Context.MeetingTask.Where(x => x.Id == id).FirstOrDefault();
            if (Task == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            _Context.MeetingTask.Remove(Task);
            _Context.SaveChanges();
            return new Success(_mapper.Map<MeetingTaskDto>(Task));
        }

        public Result Get(Guid id)
        {
            var Task = _Context.MeetingTask.Where(x => x.Id == id)
                 .Select(x => new MeetingTaskDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     MeetingId = x.MeetingId,
                     AssigneeId = x.AssigneeId,
                     Status = x.Status,
                     DueDate = x.DueDate,
                     Assignee = _mapper.Map<UserDto>(x.Assignee)
                     //Meeting = _mapper.Map<MeetingDto>(x.Meeting)
                 })
                .FirstOrDefault();
            if (Task == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            else
            {
                TaskyIntegrationService Tasky = new TaskyIntegrationService(CurrentUser, _appSettings.TaskyHost);
                string Status = Tasky.GetTaskStatus(Task.RelatedTaskId.ToString());
                if (Status != null)
                {
                    Task.Status = int.Parse(Status);
                }
                return new Success(Task);
            }
        }

        public Result GetAll(DataSource dataSource)
        {
            
            var FromDate = DateTime.Parse(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault().Value);
            dataSource.Filter.Remove(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault());

            var Tasks = _Context.Meeting
                .Where(x => x.From.Date==FromDate.Date && x.MeetingTask.Any(z => z.AssigneeId == CurrentUser.Id || z.CreatedBy == CurrentUser.Id))
                .OrderBy(x => x.From)
                .Select(x => new MeetingDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    From = x.From,
                    MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask.Where(z => z.AssigneeId == CurrentUser.Id || z.CreatedBy == CurrentUser.Id)),
                    MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name
                       }
                   }))
                })
                .AsQueryable();
            TaskyIntegrationService Tasky = new TaskyIntegrationService(CurrentUser, _appSettings.TaskyHost);

            if (Tasks != null)
            {

                foreach (var item in Tasks)
                {
                    foreach (var taskItem in item.MeetingTask)
                    {
                        string Status = Tasky.GetTaskStatus(taskItem.RelatedTaskId.ToString());
                        if (Status != null && Status != taskItem.Status.ToString())
                        {
                            // taskItem.Status = int.Parse(Status);
                            var __Task = _Context.MeetingTask.Where(x => x.MeetingId == taskItem.MeetingId && x.Id == taskItem.Id).FirstOrDefault();
                            __Task.Status = int.Parse(Status);
                            _Context.SaveChanges();
                        }
                      
                    }

                }
                return new Success(dataSource.ToResult(Tasks));
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());

        }
        public Result Update(MeetingTaskDto meetingTask)
        {
            
            var meeting = _Context.Meeting.FirstOrDefault(x => x.Id == meetingTask.MeetingId);
            if (meeting == null)
                return new Error(ErrorMsg.NotValid.ToDescriptionString());

            if (!IsValidTask(meetingTask.DueDate, meeting.From))
            {
                return new Error(ErrorMsg.TaskDueDateInPast.ToDescriptionString());
            }
            else if (!IsValidTask(meetingTask.AssigneeId, meetingTask.MeetingId))
            {
                return new Error(ErrorMsg.TaskAssigneeNotMeetingParticipant.ToDescriptionString());
            }
            else if (!IsUniqueTask(meetingTask))
            {
                return new Error(ErrorMsg.NotUnique.ToDescriptionString());
            }
            else
            {
                var MeetingTaskInDb = _Context.MeetingTask.SingleOrDefault(c => c.Id == meetingTask.Id);

                if (MeetingTaskInDb == null)
                    return new Error(ErrorMsg.NotFound.ToDescriptionString());

                _mapper.Map(meetingTask, MeetingTaskInDb);
                _Context.Entry(MeetingTaskInDb).State = EntityState.Modified;
                _Context.SaveChanges();
                return new Success(meetingTask);
            }
               
        }

        #region helpers
        private bool IsValidTask(DateTime DueDate, DateTime MeetingStart)
        {
            if (DueDate < DateTime.UtcNow || DueDate <= MeetingStart)
                return false;
            return true;
        }
        private bool IsValidTask(Guid AssigneeId,Guid MeetingId)
        {
           var Participant= _Context.Meeting.Where(x => x.Id == MeetingId &&
            x.MeetingParticipant.Any(y => y.ParticipantId == AssigneeId)).FirstOrDefault();
            return (Participant == null) ? false : true;
        }
        private bool IsUniqueTask(MeetingTaskDto meetingTask)
        {
            return !_Context.MeetingTask.Any(x => x.Id != meetingTask.Id
            && (x.Name.ToLower().Equals(meetingTask.Name.ToLower())) 
            && (x.MeetingId==meetingTask.MeetingId) && (x.AssigneeId==meetingTask.AssigneeId));
        }
        #endregion
    }
}
