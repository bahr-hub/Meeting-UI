using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Linq;
using static Shared.SystemEnums;

namespace Services
{
    public interface IMeetingTopicService : IBaseService
    {
        Result Get(Guid ID);
        Result GetAll(DataSource dataSource);
        Result Create(MeetingTopicDto MeetingTopicDto);
        Result Update(MeetingTopicDto MeetingTopicDto);
        Result Delete(Guid ID);
        Result CloseTopic(Guid ID);
    }
    public class MeetingTopicService : BaseService, IMeetingTopicService
    {

        public MeetingTopicService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { }

        public Result Get(Guid ID)
        {
            var MeetingTopic = _Context.MeetingTopic
                .Where(x => x.Id == ID && x.IsDeleted == false)
                .Select(x => new MeetingTopicDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Duration = x.Duration,
                    PresenterId = x.PresenterId,
                    FkMeetingId = x.FkMeetingId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    Presenter = new UserDto
                    {
                        Name = x.Presenter.Name
                    }
                })
                .FirstOrDefault();
            if (MeetingTopic != null)
                return new Success(MeetingTopic);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {
            var MeetingTopics = _Context.MeetingTopic
                .Where(x => x.IsDeleted == false)
                 .Select(x => new MeetingTopicDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Duration = x.Duration,
                     PresenterId = x.PresenterId,
                     FkMeetingId=x.FkMeetingId,
                     CreatedAt=x.CreatedAt,
                     CreatedBy=x.CreatedBy
                 })
                .AsQueryable();
            if (MeetingTopics != null)
                return new Success(dataSource.ToResult(MeetingTopics));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result Create(MeetingTopicDto MeetingTopicDto)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == MeetingTopicDto.FkMeetingId);
            if (meeting == null)
                return new Error(ErrorMsg.NotValid.ToDescriptionString());
            var dto = _mapper.Map<MeetingDto>(meeting);
            if (!dto.Started || !dto.NotEnded)
                return new Error(ErrorMsg.MeetingNotRunning.ToDescriptionString());
            if (!IsValidTopic(MeetingTopicDto.FkMeetingId, MeetingTopicDto.PresenterId))
                return new Error(ErrorMsg.PresenterNotParticipant.ToDescriptionString());
            if (!IsValidTopics(MeetingTopicDto))
                return new Error(ErrorMsg.DuplicateTopics.ToDescriptionString());
            else
            {
                var MeetingTopic = _mapper.Map<MeetingTopic>(MeetingTopicDto);
                _Context.MeetingTopic.Add(MeetingTopic);
                _Context.SaveChanges();
                var result = (Success)Get(MeetingTopic.Id);
                MeetingTopicDto = result.Data;
                return new Success(MeetingTopicDto, ResponseMessage.TopicCreated.ToDescriptionString());
            }
           
        }

        public Result Update(MeetingTopicDto MeetingTopicDto)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == MeetingTopicDto.FkMeetingId);
            if (meeting == null)
                return new Error(ErrorMsg.NotValid.ToDescriptionString());
            var dto = _mapper.Map<MeetingDto>(meeting);
            if (!dto.Started || !dto.NotEnded)
                return new Error(ErrorMsg.MeetingNotRunning.ToDescriptionString());
            if (!IsValidTopics(MeetingTopicDto))
                return new Error(ErrorMsg.DuplicateTopics.ToDescriptionString());

            if (IsValidTopic(MeetingTopicDto.FkMeetingId, MeetingTopicDto.PresenterId))
            {
                var MeetingTopicInDb = _Context.MeetingTopic.SingleOrDefault(c => c.Id == MeetingTopicDto.Id);

                if (MeetingTopicInDb == null)
                    return new Error(ErrorMsg.NotFound.ToDescriptionString());

                _mapper.Map(MeetingTopicDto, MeetingTopicInDb);
                _Context.Entry(MeetingTopicInDb).State = EntityState.Modified;
                _Context.SaveChanges();
                return new Success(MeetingTopicDto);
            }
            else
                return new Error(ErrorMsg.PresenterNotParticipant.ToDescriptionString());
        }

        public Result Delete(Guid ID)
        {
            var MeetingTopic = _Context.MeetingTopic
               .Where(x => x.Id == ID && x.IsDeleted != true)
               .FirstOrDefault();
            if (MeetingTopic == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            MeetingTopic.IsDeleted = true;
            _Context.SaveChanges();
            var MeetingTopicDto = _mapper.Map<MeetingTopic, MeetingTopicDto>(MeetingTopic);
            return new Success(MeetingTopicDto);
        }
        public Result CloseTopic(Guid ID)
        {
            var Topic = _Context.MeetingTopic
                .Include(x=>x.FkMeeting)
                .Where(x => x.Id == ID).FirstOrDefault();
            if (Topic == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            if (CurrentUser.Id != Topic.FkMeeting.CreatedBy)
                return new Error(ErrorMsg.NotMeetingCreator.ToDescriptionString());
            Topic.IsClosed = true;
            _Context.SaveChanges();
            return new Success(_mapper.Map<MeetingTopicDto>(Topic), ResponseMessage.TopicClosed.ToDescriptionString());
        }
        #region helper
        private bool IsValidTopic(Guid fkMeetingId, Guid presenterId)
        {
            var meeting = _Context.Meeting.Include(x=>x.MeetingParticipant).FirstOrDefault(x => x.Id == fkMeetingId);
            var dto = _mapper.Map<MeetingDto>(meeting);
            if (dto.Started && dto.NotEnded)
            {
                if (meeting.MeetingParticipant.Any(x => x.ParticipantId == presenterId))// && x.Response))
                    return true;
            }
            return false;
        }
        private bool IsValidTopics(MeetingTopicDto meetingTopic)
        {
            return !_Context.MeetingTopic.Any(x => x.Id != meetingTopic.Id 
            && (x.FkMeetingId==meetingTopic.FkMeetingId)
            && (x.PresenterId== meetingTopic.PresenterId)
            && (x.Name.ToLower().Equals(meetingTopic.Name.ToLower())) && (x.IsDeleted == false));
        }
        #endregion

    }
}
