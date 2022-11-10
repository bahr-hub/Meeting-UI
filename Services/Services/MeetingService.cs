using AutoMapper;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Utils;
using Model.DTO;
using Model.Models;
using Services.Services;
using Shared;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using static Shared.SystemEnums;

namespace Services
{
    public interface IMeetingService : IBaseService
    {
        Result Get(Guid ID);
        Result GetAll(DataSource dataSource);
        Result GetFiltredMeeting(DataSource dataSource);
        Result GetAllLite();
        Result GetPreviousMeetings(DataSource dataSource);
        Result GetUpcomingMeetings(DataSource dataSource);
        Result GetCurrentMeetings(DataSource dataSource);
        Result GetAllMeetings(DataSource dataSource, bool isGatting = false);
        Result Create(MeetingDto Meeting);
        Result Update(MeetingDto Meeting);
        Result ChangeMeetingStatus(Guid ID, int Status);
        Result Delete(Guid ID);
        Result GetParticipants(DateTime meetingStart, DateTime meetingEnd, Guid? meetingId = null);
        Result GetLocations(DateTime meetingFrom, DateTime meetingTo, Guid? meetingId = null);

        Result GetJoinedParticipants(Guid meetingId);
        Result DeleteParticipant(Guid ID, Guid MeetingID);
        Result Start(Guid iD);
        Result End(Guid iD);
        Result Join(Guid MeetingId);
        Result Accept(Guid iD);
        Result Decline(Guid iD);
        Result DeclineProposal(Guid iD, Guid participant);
        Result AddNote(Guid iD, string note);
        Result SendMeetingCreationEmail(MeetingDto meetingDto);
        Result SendMeetingUpdatingEmail(MeetingDto meetingDto);
        Result GetAll(DateTime start, DateTime end);
        Result SendMeetingDeleteEmail(MeetingDto meetingDto);
        //Result SendMeetingStatusEmail(MeetingDto meetingDto);
        Result SendMeetingStartedEmail(MeetingDto meetingDto);
        Result SendMeetingEndedEmail(MeetingDto meetingDto);
        Result SendMeetingAcceptingEmail(MeetingDto meetingDto);
        Result SendMeetingDeclineEmail(MeetingDto meetingDto);
        Result AddParticipants(Guid ID, List<MeetingParticipantDto> participantDtos);
        Result Postpone(Guid ID, DateTime PostponedTo);
        Result PurposeNewTime(Guid ID, DateTime PurposedTo);
        Result AcceptProposal(Guid meetingId, DateTime proposedTime);


    }
    public class MeetingService : BaseService, IMeetingService
    {
        IAccountService _accountService;
        IGoogleCalender _googleCalender;
        public MeetingService(IGoogleCalender googleCalender, IOptions<AppSettings> AppSettings, IAccountService accountService, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env)
        {
            _accountService = accountService;
            _googleCalender = googleCalender;


        }
        public Result Get(Guid ID)
        {
            var Meeting = _Context.Meeting
                .Where(x => x.Id == ID && x.IsDeleted == false)
                .Include(x => x.MeetingTag)
                .Select(x => new MeetingDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    LocationId = x.LocationId,
                    ProjectId = x.ProjectId,
                    From = ToTimeZone(x.From),
                    To = ToTimeZone(x.To),
                    Notes = x.Notes,
                    StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                    EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ExternalToken = x.ExternalToken,
                    UpdatedBy = x.UpdatedBy,
                    PreviousMeetingID = x.PreviousMeetingID,
                    MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                    MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                    MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask),
                    Location = _mapper.Map<LocationDto>(x.Location),
                    CreatedByNavigation = new UserDto
                    {
                        Email = x.CreatedByNavigation.Email,
                        FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile)
                    },
                    MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                           Email = y.Participant.Email,
                           ImageUrl = y.Participant.ImageUrl,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   }))
                })
                .FirstOrDefault();

            TaskyIntegrationService Tasky = new TaskyIntegrationService(CurrentUser, _appSettings.TaskyHost);

            foreach (var item in Meeting.MeetingTask)
            {
                string Status = Tasky.GetTaskStatus(item.RelatedTaskId.ToString());
                if (Status != null && Status != item.Status.ToString())
                {
                    // taskItem.Status = int.Parse(Status);
                    var __Task = _Context.MeetingTask.Where(x => x.MeetingId == item.MeetingId && x.Id == item.Id).FirstOrDefault();
                    __Task.Status = int.Parse(Status);
                    _Context.SaveChanges();
                    item.Status = int.Parse(Status);
                }


            }
            if (Meeting != null)

                return new Success(Meeting);
            else return new Error(ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result GetAll(DataSource dataSource)
        {

            var Meetings = _Context.Meeting
                .Where(x => x.IsDeleted == false)
                 .Select(x => new MeetingDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     LocationId = x.LocationId,
                     ProjectId = x.ProjectId,
                     From = ToTimeZone(x.From),
                     To = ToTimeZone(x.To),
                     Notes = x.Notes,
                     StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                     EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                     CreatedBy = x.CreatedBy,
                     CreatedAt = x.CreatedAt,
                     UpdatedAt = x.UpdatedAt,
                     UpdatedBy = x.UpdatedBy,
                     MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)),
                     MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                     MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   })),
                     CreatedByNavigation = _mapper.Map<UserDto>(x.CreatedByNavigation)
                 })
                .AsQueryable();
            if (Meetings != null)

                return new Success(dataSource.ToResult(Meetings));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetAllLite()
        {
            var meetingList = _Context.Meeting.Where(x => (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null))
                .Select(x => new MeetingDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).AsEnumerable();
            if (meetingList != null)
                return new Success(meetingList);
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result GetFiltredMeeting(DataSource dataSource)
        {
            var meetingList = _Context.Meeting
                .Where(x => x.IsDeleted == false && (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null))
             .Select(x => new MeetingDto
             {
                 Id = x.Id,
                 Name = x.Name,
                 From = ToTimeZone(x.From),
                 StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                 EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                 LocationId = x.LocationId,
                 CreatedBy = x.CreatedBy
             }).AsQueryable();

            if (meetingList != null)
                return new Success(dataSource.ToResult(meetingList));
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetAll(DateTime start, DateTime end)
        {
            var Meetings = _Context.Meeting
                .Where(x => x.IsDeleted == false && ((x.From >= start && x.From <= end)
                || (x.To >= start && x.To <= end))
              && (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null)
                && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id))
                 .Select(x => new MeetingDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     LocationId = x.LocationId,
                     ProjectId = x.ProjectId,
                     From = ToTimeZone(x.From),
                     To = ToTimeZone(x.To),
                     Notes = x.Notes,
                     StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                     EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                     CreatedBy = x.CreatedBy,
                     CreatedAt = x.CreatedAt,
                     UpdatedAt = x.UpdatedAt,
                     UpdatedBy = x.UpdatedBy,
                     MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)),
                     MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                     MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   })),
                     CreatedByNavigation = _mapper.Map<UserDto>(x.CreatedByNavigation)
                 })
                .AsQueryable();
            if (Meetings != null)
            {
                return new Success(Meetings.Where(x => x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null));
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetPreviousMeetings(DataSource dataSource)
        {
            var FromDate = DateTime.Parse(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault().Value);
            dataSource.Filter.Remove(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault());

            var Meetings = _Context.Meeting
                .Where(x => x.IsDeleted == false
                && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
                && (FromDate.Date == x.From.Date)
               && ((x.EndedAt != null || (x.StartedAt == null && x.To < DateTime.UtcNow)))
                 )
                 .Select(x => new MeetingDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     LocationId = x.LocationId,
                     ProjectId = x.ProjectId,
                     From = ToTimeZone(x.From),
                     To = ToTimeZone(x.To),
                     Notes = x.Notes,
                     StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                     EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                     CreatedBy = x.CreatedBy,
                     CreatedAt = x.CreatedAt,
                     UpdatedAt = x.UpdatedAt,
                     UpdatedBy = x.UpdatedBy,
                     MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                     MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                     MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask),
                     Location = _mapper.Map<LocationDto>(x.Location),
                     CreatedByNavigation = new UserDto
                     {
                         Email = x.CreatedByNavigation.Email,
                         FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile)
                     },
                     MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   }))
                 })
                .AsQueryable();
            if (Meetings != null)
            {

                var res = dataSource.ToResult(Meetings);
                return new Success(res);
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetUpcomingMeetings(DataSource dataSource)
        {
            var FromDate = DateTime.Parse(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault().Value);
            dataSource.Filter.Remove(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault());

            var Meetings = _Context.Meeting
                .Where(x => x.IsDeleted == false
                && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
                && ((FromDate.Date == x.From.Date)
                && (x.StartedAt == null && x.From > DateTime.UtcNow))
               ).OrderBy(x => x.From)
                 .Select(x => new MeetingDto
                 {
                     Id = x.Id,
                     Name = x.Name,
                     LocationId = x.LocationId,
                     ProjectId = x.ProjectId,
                     From = ToTimeZone(x.From),
                     To = ToTimeZone(x.To),
                     Notes = x.Notes,
                     StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                     EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                     CreatedBy = x.CreatedBy,
                     CreatedAt = x.CreatedAt,
                     UpdatedAt = x.UpdatedAt,
                     UpdatedBy = x.UpdatedBy,
                     MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                     MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                     MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask),
                     Location = _mapper.Map<LocationDto>(x.Location),
                     CreatedByNavigation = new UserDto
                     {
                         Email = x.CreatedByNavigation.Email,
                         FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile)
                     },
                     MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   }))
                 })
                .AsQueryable();
            if (Meetings != null)
            {
                var res = dataSource.ToResult(Meetings);
                return new Success(res);
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public Result GetCurrentMeetings(DataSource dataSource)
        {
            var FromDate = DateTime.Parse(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault().Value);
            dataSource.Filter.Remove(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault());

            var Meetings = _Context.Meeting
                    .Where(x => x.IsDeleted == false && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
                 && (FromDate.Date == DateTime.UtcNow.Date
                 && x.EndedAt == null
                 && ((x.From <= DateTime.UtcNow && DateTime.UtcNow <= x.To)
                 || (x.StartedAt != null && x.StartedAt <= DateTime.UtcNow)))
                 )
                .Select(x => new MeetingDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    LocationId = x.LocationId,
                    ProjectId = x.ProjectId,
                    From = ToTimeZone(x.From),
                    To = ToTimeZone(x.To),
                    Notes = x.Notes,
                    StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                    EndedAt = x.EndedAt != null ? ToTimeZone(x.EndedAt.GetValueOrDefault()) : x.EndedAt,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                    MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                    MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask),
                    Location = _mapper.Map<LocationDto>(x.Location),
                    CreatedByNavigation = new UserDto
                    {
                        Email = x.CreatedByNavigation.Email,
                        FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile)
                    },
                    MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   }))
                })
                .AsQueryable();
            if (Meetings != null)
            {
                var res = dataSource.ToResult(Meetings);
                return new Success(res);
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        public Result GetAllMeetings(DataSource dataSource, bool isGatting )
        {


            var _user = CurrentUser;
            List<Response<MeetingDto>> meetings = new List<Response<MeetingDto>>();
            var fromDate = DateTime.Parse(dataSource.Filter.Where(x => x.Key == "From").FirstOrDefault().Value);
            string meetingName = dataSource.Filter.Where(x => x.Key == "meetingName").FirstOrDefault().Value;

            if (!isGatting )
            {
                //New Code Here
                meetings.Add(GetCurrentMeetings(fromDate));
                meetings.Add(GetUpcomingMeetings(fromDate));
                meetings.Add(GetPreviousMeetings(fromDate));
            }

            meetings.Add(GetAllMeetingsForGatting(fromDate));

           

            if (meetings != null)
            {
                return new Success(meetings);
            }
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }

        public bool DatesAreInTheSameWeek(DateTime date1, DateTime date2)
        {
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            var d1 = date1.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date1));
            var d2 = date2.Date.AddDays(-1 * (int)cal.GetDayOfWeek(date2));

            return d1 == d2;
        }
        private Response<MeetingDto> GetAllMeetingsForGatting(DateTime fromDate)
        {
            var upcomingMeetings = _Context.Meeting
             .Where(x => x.IsDeleted == false && (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null)
             && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
             && DatesAreInTheSameWeek(x.From.Date, DateTime.UtcNow)/*(x.StartedAt == null && x.From > DateTime.UtcNow)*/
            )
            .Select(x => new MeetingDto
            {
                Id = x.Id,
                Name = x.Name,
                From = ToTimeZone(x.From),
                To = ToTimeZone(x.To),
                PreviousMeetingID = x.PreviousMeetingID != null ? x.PreviousMeetingID : Guid.Empty,
                CreatedBy = x.CreatedBy,
                StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                       }
                   }))
            }).OrderBy(x => x.From).AsQueryable();
            return new Response<MeetingDto>()
            {
                Meetings = upcomingMeetings.ToList(),
                Type = MeetingTypeEnum.Upcoming
            };
        }

        private Response<MeetingDto> GetUpcomingMeetings(DateTime fromDate)
        {
            var upcomingMeetings = _Context.Meeting
             .Where(x => x.IsDeleted == false && (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null)
             && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
             && ((fromDate.Date == x.From.Date)
             && (x.StartedAt == null && x.From > DateTime.UtcNow))
            )
            .Select(x => new MeetingDto
            {
                Id = x.Id,
                Name = x.Name,
                From = ToTimeZone(x.From),
                To = ToTimeZone(x.To),
                PreviousMeetingID = x.PreviousMeetingID != null ? x.PreviousMeetingID : Guid.Empty,
                CreatedBy = x.CreatedBy,
                StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                       }
                   }))
            }).OrderBy(x => x.From).AsQueryable();
            return new Response<MeetingDto>()
            {
                Meetings = upcomingMeetings.ToList(),
                Type = MeetingTypeEnum.Upcoming
            };
        }
        

        private Response<MeetingDto> GetCurrentMeetings(DateTime fromDate)
        {
            var currentMeetings = _Context.Meeting
                   .Where(x => x.IsDeleted == false && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
                   && (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null)
                && (fromDate.Date == DateTime.UtcNow.Date
                && x.EndedAt == null
                && ((x.From <= DateTime.UtcNow && DateTime.UtcNow <= x.To)
                || (x.StartedAt != null && x.StartedAt <= DateTime.UtcNow)))
                )
                            .Select(x => new MeetingDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                CreatedBy = x.CreatedBy,
                                From = ToTimeZone(x.From),
                                To = ToTimeZone(x.To),
                                PreviousMeetingID = x.PreviousMeetingID != null ? x.PreviousMeetingID : Guid.Empty,
                                StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                                MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                                MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Response = y.Response,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                       }
                   }))
                            }).OrderBy(x => x.From).AsQueryable();
            return new Response<MeetingDto>()
            {
                Meetings = currentMeetings.ToList(),
                Type = MeetingTypeEnum.Current
            };
        }
        private Response<MeetingDto> GetPreviousMeetings(DateTime fromDate)
        {
            var previousMeetings = _Context.Meeting
              .Where(x => x.IsDeleted == false
               && (x.LocationId == CurrentUser.LocationID || CurrentUser.LocationID == null)
              && x.MeetingParticipant.Any(z => z.ParticipantId == CurrentUser.Id)
              && (fromDate.Date == x.From.Date)
             && ((x.EndedAt != null || (x.StartedAt == null && x.To < DateTime.UtcNow)))
               )
                               .Select(x => new MeetingDto
                               {
                                   Id = x.Id,
                                   Name = x.Name,
                                   From = ToTimeZone(x.From),
                                   To = ToTimeZone(x.To),
                                   PreviousMeetingID = x.PreviousMeetingID != null ? x.PreviousMeetingID : Guid.Empty,
                                   CreatedBy = x.CreatedBy,
                                   StartedAt = x.StartedAt != null ? ToTimeZone(x.StartedAt.GetValueOrDefault()) : x.StartedAt,
                                   MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
                    .Select(y => new MeetingTopicDto
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Duration = y.Duration,
                        PresenterId = y.PresenterId,
                        CreatedAt = y.CreatedAt,
                        CreatedBy = y.CreatedBy,
                        IsClosed = y.IsClosed,
                        Presenter = new UserDto
                        {
                            Name = y.Presenter.Name
                        }

                    })
                    ),
                                   MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       JoinedMeeting = y.JoinedMeeting,
                       JoinedMeetingTime = y.JoinedMeetingTime,
                       Response = y.Response,
                       Participant = new UserDto
                       {
                           Id = y.Participant.Id,
                           Name = y.Participant.Name,
                       }
                   }))
                               }).OrderBy(x => x.From).AsQueryable();
            return new Response<MeetingDto>()
            {
                Meetings = previousMeetings.ToList(),
                Type = MeetingTypeEnum.Previous
            };
        }
        public Result Create(MeetingDto MeetingDto)
        {
            if (MeetingDto.From > MeetingDto.To)
                return new Error(ErrorMsg.MeetingEndEarlierThanMeetingStart.ToDescriptionString());
            if (!IsValidMeeting(MeetingDto.From, MeetingDto.To, MeetingDto.MeetingParticipant.ToList()))
                return new Error(ErrorMsg.NotValid.ToDescriptionString());
            if (!IsValidMeeting(MeetingDto.From, MeetingDto.To))
                return new Error(ErrorMsg.MeetingInPast.ToDescriptionString());
            if (!IsValidMeeting(MeetingDto.From, MeetingDto.To, MeetingDto.MeetingTopic.Sum(x => x.Duration)))
                return new Error(ErrorMsg.TopicsDurationGreaterThanMeetingDuration.ToDescriptionString());
            if (MeetingDto.MeetingParticipant.Count == 0)
                return new Error(ErrorMsg.MeetingMustHaveParticipants.ToDescriptionString());
            if (MeetingDto.MeetingTopic.Count == 0)
                return new Error(ErrorMsg.MeetingMustHaveTopics.ToDescriptionString());

            var DuplicateParticipants = MeetingDto.MeetingParticipant.GroupBy(x => x.ParticipantId)
              .Where(g => g.Count() > 1).ToList();
            if (DuplicateParticipants.Count > 0)
                return new Error(ErrorMsg.DuplicateParticipants.ToDescriptionString());

            var DuplicateTopics = MeetingDto.MeetingTopic.GroupBy(x => new { x.PresenterId, Name = x.Name.ToLower() })
               .Where(g => g.Count() > 1).ToList();
            if (DuplicateTopics.Count > 0)
                return new Error(ErrorMsg.DuplicateTopics.ToDescriptionString());
            foreach (MeetingTopicDto D in MeetingDto.MeetingTopic)
            {
                if (D.Name == "")
                    return new Error(ErrorMsg.NameIsRequired.ToDescriptionString());
            }

            var PresentersNotParticipants =
             MeetingDto.MeetingTopic.Where(x => !MeetingDto.MeetingParticipant.Any(y => x.PresenterId == y.ParticipantId)).ToList();

            if (PresentersNotParticipants != null && PresentersNotParticipants.Count != 0)
                return new Error(ErrorMsg.PresenterNotParticipant.ToDescriptionString());

            var Meeting = _mapper.Map<Meeting>(MeetingDto);
            var MeetingTopics = new List<MeetingTopic>(Meeting.MeetingTopic);
            var MeetingParticipants = new List<MeetingParticipant>(Meeting.MeetingParticipant);
            var MeetingTags = new List<MeetingTag>(Meeting.MeetingTag);

            Meeting.MeetingTopic.Clear();
            Meeting.MeetingParticipant.Clear();
            Meeting.MeetingTag.Clear();
            _Context.Meeting.Add(Meeting);
            _Context.SaveChanges();
            //var meeting = _Context.Meeting.SingleOrDefault(m => m.Id.Equals(Meeting.Id));
             Meeting.ExternalToken = this.GenerateToken(Meeting.Id);
            _Context.Entry(Meeting).State = EntityState.Modified;
            _Context.SaveChanges();

            if (!MeetingParticipants.Any(p => p.ParticipantId == Meeting.CreatedBy))
            {
                MeetingParticipants.Add(
              new MeetingParticipant
              {
                  ParticipantId = Meeting.CreatedBy
              });
            }

            foreach (MeetingTopic topic in MeetingTopics)
            {
                topic.FkMeetingId = Meeting.Id;
                _Context.MeetingTopic.Add(topic);
                _Context.SaveChanges();
            }
            Meeting.MeetingTopic = MeetingTopics;
            foreach (MeetingParticipant MeetingParticipant in MeetingParticipants)
            {
                MeetingParticipant.MeetingId = Meeting.Id;
                _Context.MeetingParticipant.Add(MeetingParticipant);
                _Context.SaveChanges();
            }
            Meeting.MeetingParticipant = MeetingParticipants;
            foreach (MeetingTag tag in MeetingTags)
            {
                tag.MeetingId = Meeting.Id;
                _Context.MeetingTag.Add(tag);
                _Context.SaveChanges();
            }
            Meeting.MeetingTag = MeetingTags;
            //Instead of mapping to Dto To include profile and participent profile to be used in email
            var result = (Success)Get(Meeting.Id);
            MeetingDto = result.Data;
            try
            {
                this.CreateNotifications(MeetingDto, " invited you to new meeting " + MeetingDto.Name + ".", " دعاك لحضور اجتماع جديد " + MeetingDto.Name + ".");
            }
            catch (Exception e)
            {

                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            try
            {

                this.SendMeetingCreationEmail(MeetingDto);
                _googleCalender.AddMeeting(MeetingDto);


            }
            catch (Exception e)
            {

                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            return new Success(MeetingDto, ResponseMessage.MeetingCreated.ToDescriptionString());

        }
        public Result Update(MeetingDto MeetingDto)
        {
            if (MeetingDto.From > MeetingDto.To)
                return new Error(ErrorMsg.MeetingEndEarlierThanMeetingStart.ToDescriptionString());
            if (!IsValidMeeting(MeetingDto.From, MeetingDto.To, MeetingDto.MeetingParticipant.ToList()))
                return new Error(ErrorMsg.NotValid.ToDescriptionString());
            if (!IsValidMeeting(MeetingDto.From, MeetingDto.To))
                return new Error(ErrorMsg.MeetingInPast.ToDescriptionString());
            if (!IsValidMeeting(MeetingDto.From, MeetingDto.To, MeetingDto.MeetingTopic.Sum(x => x.Duration)))
                return new Error(ErrorMsg.TopicsDurationGreaterThanMeetingDuration.ToDescriptionString());
            if (MeetingDto.MeetingParticipant.Count == 0)
                return new Error(ErrorMsg.MeetingMustHaveParticipants.ToDescriptionString());
            if (MeetingDto.MeetingTopic.Count == 0)
                return new Error(ErrorMsg.MeetingMustHaveTopics.ToDescriptionString());

            var DuplicateParticipants = MeetingDto.MeetingParticipant.GroupBy(x => x.ParticipantId)
             .Where(g => g.Count() > 1).ToList();
            if (DuplicateParticipants.Count > 0)
                return new Error(ErrorMsg.DuplicateParticipants.ToDescriptionString());

            var DuplicateTopics = MeetingDto.MeetingTopic.GroupBy(x => new { x.PresenterId, Name = x.Name.ToLower() })
           .Where(g => g.Count() > 1).ToList();
            if (DuplicateTopics.Count > 0)
                return new Error(ErrorMsg.DuplicateTopics.ToDescriptionString());

            var MeetingInDb = _Context.Meeting.FirstOrDefault(c => c.Id == MeetingDto.Id);

            if (MeetingInDb == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());

            var PresentersNotParticipants =
               MeetingDto.MeetingTopic.Where(x => !MeetingDto.MeetingParticipant.Any(y => x.PresenterId == y.ParticipantId)).ToList();

            if (PresentersNotParticipants != null && PresentersNotParticipants.Count != 0)
                return new Error(ErrorMsg.PresenterNotParticipant.ToDescriptionString());

            _Context.MeetingParticipant.RemoveRange(_Context.MeetingParticipant
                   .Where(x => x.MeetingId == MeetingInDb.Id && !MeetingDto.MeetingParticipant.Any(y => y.Id == x.Id)));

            _Context.SaveChanges();

            if (!MeetingDto.MeetingParticipant.Any(p => p.ParticipantId == MeetingDto.CreatedBy))
            {
                MeetingDto.MeetingParticipant.Add(
              new MeetingParticipantDto
              {
                  ParticipantId = MeetingDto.CreatedBy
              });
            }

            //_mapper.Map(MeetingDto.MeetingParticipant, MeetingInDb.MeetingParticipant);
            MeetingParticipant participant;
            foreach (var item in MeetingDto.MeetingParticipant)
            {
                participant = _mapper.Map<MeetingParticipant>(item);
                participant.MeetingId = MeetingInDb.Id;
                if (item.Id != new Guid())
                    _Context.Entry(participant).State = EntityState.Modified;
                else
                    _Context.Entry(participant).State = EntityState.Added;
                _Context.SaveChanges();
            }
            _Context.SaveChanges();




            _Context.MeetingTopic.RemoveRange(_Context.MeetingTopic
                 .Where(x => x.FkMeetingId == MeetingInDb.Id && !MeetingDto.MeetingTopic.Any(y => y.Id == x.Id)).ToList());

            _Context.SaveChanges();
            // _mapper.Map(MeetingDto.MeetingTopic, MeetingInDb.MeetingTopic);
            MeetingTopic topic;
            foreach (var item in MeetingDto.MeetingTopic)
            {
                topic = _mapper.Map<MeetingTopic>(item);
                topic.FkMeetingId = MeetingInDb.Id;
                if (item.Id != new Guid())
                    _Context.Entry(topic).State = EntityState.Modified;
                else
                    _Context.Entry(topic).State = EntityState.Added;
                _Context.SaveChanges();
            }
            _Context.SaveChanges();


            _Context.MeetingTag.RemoveRange(_Context.MeetingTag
                .Where(x => x.MeetingId == MeetingInDb.Id && !MeetingDto.MeetingTag.Any(y => y.Id == x.Id)).ToList());

            _Context.SaveChanges();

            // _mapper.Map(MeetingDto.MeetingTag, MeetingInDb.MeetingTag);
            MeetingTag tag;
            var trackedTags = _Context.ChangeTracker.Entries<MeetingTag>().ToList();
            foreach (var entry in trackedTags)
                entry.State = EntityState.Detached;
            foreach (var item in MeetingDto.MeetingTag)
            {
                tag = _mapper.Map<MeetingTag>(item);
                tag.MeetingId = MeetingInDb.Id;


                if (item.Id != new Guid())
                    _Context.Entry(tag).State = EntityState.Modified;
                else
                    _Context.Entry(tag).State = EntityState.Added;
                _Context.SaveChanges();
            }
            _Context.SaveChanges();

            MeetingDto.MeetingParticipant = null;
            MeetingDto.MeetingTag = null;
            MeetingDto.MeetingTopic = null;
            MeetingInDb.Name = MeetingDto.Name;
            MeetingInDb.LocationId = MeetingDto.LocationId;
            MeetingInDb.ProjectId = MeetingDto.ProjectId;
            MeetingInDb.PreviousMeetingID = MeetingDto.PreviousMeetingID;
            MeetingInDb.From = MeetingDto.From;
            MeetingInDb.To = MeetingDto.To;

            _Context.SaveChanges();

            //Instead of mapping to Dto To include profile and participent profile to be used in email
            var result = (Success)Get(MeetingInDb.Id);
            MeetingDto = result.Data;
            try
            {
                this.CreateNotifications(MeetingDto, " , updated " + MeetingDto.Name + " meeting.", ", حدث " + MeetingDto.Name + " اجتماع.");
            }
            catch (Exception e)
            {
                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            try
            {
                this.SendMeetingUpdatingEmail(MeetingDto);
            }
            catch (Exception e)
            {
                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            return new Success(MeetingDto, ResponseMessage.MeetingUpdated.ToDescriptionString());
        }
        public Result ChangeMeetingStatus(Guid ID, int Status)
        {
            var Meeting = _Context.Meeting
                .Where(x => x.Id == ID)
                .FirstOrDefault();
            if (Meeting == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Meeting.Status = Status;
            _Context.SaveChanges();
            //var MeetingDto = _mapper.Map<Meeting, MeetingDto>(Meeting);
            //Instead of mapping to Dto To include profile and participent profile to be used in email
            var result = (Success)Get(Meeting.Id);
            var MeetingDto = result.Data;
            // this.SendMeetingStatusEmail(MeetingDto);
            return new Success(MeetingDto);

        }
        public Result Delete(Guid ID)
        {
            var Meeting = _Context.Meeting
               .Where(x => x.Id == ID && x.IsDeleted != true)
               .FirstOrDefault();
            if (Meeting == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            Meeting.IsDeleted = true;
            _Context.SaveChanges();
            //var MeetingDto = _mapper.Map<Meeting, MeetingDto>(Meeting);
            //Instead of mapping to Dto To include profile and participent profile to be used in email
            var MeetingDto = _Context.Meeting
                .Where(x => x.Id == ID)
                .Select(x => new MeetingDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    LocationId = x.LocationId,
                    ProjectId = x.ProjectId,
                    From = x.From,
                    To = x.To,
                    Notes = x.Notes,
                    Location = _mapper.Map<LocationDto>(x.Location),
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic),
                    MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
                    MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask),
                    CreatedByNavigation = new UserDto
                    {
                        Email = x.CreatedByNavigation.Email,
                        FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile)
                    },
                    MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       Participant = new UserDto
                       {
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
                       }
                   }))
                })
                .FirstOrDefault();
            try
            {
                this.CreateNotifications(MeetingDto, " deleted the meeting " + MeetingDto.Name + ".", " حذف الاجتماع " + ".");
            }
            catch (Exception e)
            {
                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            try
            {
                this.SendMeetingDeleteEmail(MeetingDto);
            }
            catch (Exception e)
            {
                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            return new Success(MeetingDto, ResponseMessage.MeetingDeleted.ToDescriptionString());
        }
        public Result AddParticipants(Guid ID, List<MeetingParticipantDto> participantDtos)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(ErrorMsg.NotValid.ToDescriptionString());
            var MeetingDto = _mapper.Map<MeetingDto>(meeting);
            List<MeetingParticipantDto> AllParticipants = MeetingDto.MeetingParticipant.ToList();
            AllParticipants.AddRange(participantDtos);
            var DuplicateParticipants = AllParticipants.GroupBy(x => x.ParticipantId)
             .Where(g => g.Count() > 1).ToList();
            if (DuplicateParticipants.Count > 0)
                return new Error(ErrorMsg.DuplicateParticipants.ToDescriptionString());
            MeetingParticipant participant;
            foreach (var item in participantDtos)
            {
                participant = _mapper.Map<MeetingParticipant>(item);
                participant.MeetingId = MeetingDto.Id;
                if (item.Id != new Guid())
                    _Context.Entry(participant).State = EntityState.Modified;
                else
                    _Context.Entry(participant).State = EntityState.Added;
                _Context.SaveChanges();
            }
            _Context.SaveChanges();

            var result = (Success)Get(ID);
            MeetingDto = result.Data;
            return new Success(MeetingDto.MeetingParticipant, ResponseMessage.ParticipantAdded.ToDescriptionString());

        }
        public Result GetParticipants(DateTime meetingStart, DateTime meetingEnd, Guid? meetingId = null)
        {
            var Participants = _Context.User
            .Include(x => x.MeetingParticipant)
            .Where(x => x.IsDeleted == false && x.IsActive == true && (x.MeetingParticipant.Count == 0
                            || !x.MeetingParticipant
                            .Any(y => (y.MeetingId != meetingId || meetingId == null)
                            && (y.Meeting.IsDeleted == false) &&
                            (y.Response == true || y.Response == null)
                            && DateTimeIntervalIntersection(meetingStart, meetingEnd, y.Meeting.From, y.Meeting.To))))
          .Include(x => x.VacationFkUser)
          .Where(x => (x.VacationFkUser.Count == 0 || x.VacationFkUser
                          .Any(y =>
                           !DateTimeIntervalIntersection(meetingStart, meetingEnd, y.StartDate, y.EndDate))))
          .Select(x => new UserDto
          {
              Id = x.Id,
              Name = x.Name,
              Email = x.Email,
              Mobile = x.Mobile,
              FkUserProfile = _mapper.Map<UserProfileDto>(x.FkUserProfile),
              UserRole = _mapper.Map<ICollection<UserRoleDto>>(x.UserRole),
              FkUserConfiguration = _mapper.Map<UserConfigurationDto>(x.FkUserConfiguration),
              IsActive = x.IsActive
          }).ToList();
            return new Success(Participants);


        }

        public Result GetLocations(DateTime meetingFrom, DateTime meetingTo, Guid? meetingId = null)
        {
            //var locations = _Context.Location.Any(y =>
            //                     !DateTimeIntervalIntersection(meetingStart, meetingEnd, y., y.EndDate)).AsQueryable();

            //var locations = _Context.Location.Where(x => x.Meeting.Any(y => y.From != meetingFrom && y.To != meetingTo)).AsQueryable();
            //return new Success(locations);
            var locations = _Context.Location
               .Include(x => x.Meeting)
               .Where(x => x.Meeting.Count == 0

                                || !x.Meeting
                               .Any(y => (y.Id != meetingId || meetingId == null) && DateTimeIntervalIntersection(meetingFrom, meetingTo, y.From, y.To)))
               .Select(x => new LocationDto
               {
                   Id = x.Id,
                   Name = x.Name,
                   NameAr = x.NameAr,
               }).Where(l => (l.Id == CurrentUser.LocationID || CurrentUser.LocationID == null)).ToList();

            return new Success(locations);

        }

        public Result DeleteParticipant(Guid ParticipantID, Guid MeetingID)
        {

            var meeting = _Context.Meeting
                .Include(x => x.MeetingTopic)
                .Include(x => x.MeetingParticipant)
                .Where(x => x.Id == MeetingID).FirstOrDefault();
            var MeetingDto = _mapper.Map<MeetingDto>(meeting);
            var PresentersNotParticipants =
             MeetingDto.MeetingTopic.Where(x => x.PresenterId == ParticipantID).ToList();

            if (PresentersNotParticipants != null && PresentersNotParticipants.Count != 0)
                return new Error(ErrorMsg.PresenterNotParticipant.ToDescriptionString());
            if (meeting.MeetingParticipant.Count == 1
                && meeting.MeetingParticipant.Where(x => x.ParticipantId == ParticipantID).FirstOrDefault() != null)
                return new Error(ErrorMsg.MeetingMustHaveParticipants.ToDescriptionString());
            _Context.MeetingParticipant.Remove(_Context.MeetingParticipant
                .Where(x => x.ParticipantId == ParticipantID && x.MeetingId == MeetingID).FirstOrDefault());

            _Context.SaveChanges();

            var result = (Success)Get(MeetingID);
            MeetingDto = result.Data;
            return new Success(MeetingDto.MeetingParticipant, ResponseMessage.ParticipantRemoved.ToDescriptionString());

        }
        public Result Start(Guid ID)
        {
            var meeting = _Context.Meeting.FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.NotFound.ToDescriptionString());
            if (meeting.StartedAt != null)
                return new Error(SystemEnums.ErrorMsg.MeetingAlreadyStarted.ToDescriptionString());
            if (meeting.EndedAt != null)
                return new Error(SystemEnums.ErrorMsg.MeetingAlreadyEnded.ToDescriptionString());
            if (!(ToTimeZone(meeting.From.Date) <= ToTimeZone(DateTime.UtcNow.Date))
                || !(ToTimeZone(DateTime.UtcNow.Date) <= ToTimeZone(meeting.To.Date)))
                return new Error(SystemEnums.ErrorMsg.CantStart_OutOfMeetingInterval.ToDescriptionString());
            if (CurrentUser.Id != meeting.CreatedBy)
                return new Error(SystemEnums.ErrorMsg.OnlyCreatorCanStart.ToDescriptionString());
            meeting.StartedAt = DateTime.UtcNow;

            _Context.SaveChanges();

            //Instead of mapping to Dto To include profile and participent profile to be used in email
            var result = (Success)Get(meeting.Id);
            var MeetingDto = result.Data;
            //MeetingDto.From = ToTimeZone(meeting.From);//Convert dateFrom to timezone and pass it to front end 
            //MeetingDto.To = ToTimeZone(meeting.To);
            this.CreateNotifications(MeetingDto, " started the meeting.", " بدأ الاجتماع. ");
            this.CreateNotifications(MeetingDto, " , the meeting " + MeetingDto.Name + " started.", " , الاجتماع " + MeetingDto.Name + " بدأ.");

            this.SendMeetingStartedEmail(MeetingDto);
            return new Success(MeetingDto, ResponseMessage.MeetingStarted.ToDescriptionString());
        }

        public Result End(Guid ID)
        {
            var meeting = _Context.Meeting.FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.NotFound.ToDescriptionString());
            if (meeting.StartedAt == null)
                return new Error(SystemEnums.ErrorMsg.MeetingNotStartedYet.ToDescriptionString());
            if (meeting.EndedAt != null)
                return new Error(SystemEnums.ErrorMsg.MeetingAlreadyEnded.ToDescriptionString());

            meeting.EndedAt = DateTime.UtcNow;
            _Context.SaveChanges();

            //Instead of mapping to Dto To include profile and participent profile to be used in email
            var result = (Success)Get(meeting.Id);
            var MeetingDto = result.Data;
            this.CreateNotifications(MeetingDto, " , the meeting " + MeetingDto.Name + " Ended.", " , الاجتماع " + MeetingDto.Name + " أنتهى.");
            this.SendMeetingEndedEmail(MeetingDto);
            return new Success(MeetingDto, ResponseMessage.MeetingEnded.ToDescriptionString());
        }

        public Result Postpone(Guid ID, DateTime PostponedTo)
        {
            var meeting = _Context.Meeting.Where(x => x.Id == ID).FirstOrDefault();
            if (meeting == null)
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            else
            {
                var duration = meeting.To - meeting.From;
                meeting.PostponedTo = PostponedTo;
                meeting.From = PostponedTo;
                meeting.To = PostponedTo + duration;
                _Context.SaveChanges();
                this.CreateNotifications(_mapper.Map<MeetingDto>(meeting), " , the meeting " + meeting.Name + " started.", " الاجتماع " + meeting.Name + " بدأ.");
                return new Success(_mapper.Map<MeetingDto>(meeting), ResponseMessage.MeetingPostponed.ToDescriptionString());
            }
        }

        public Result Accept(Guid ID)
        {

            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.NotFound.ToDescriptionString());

            //to-do create 3 function:
            // isPrivous()
            // isCurrent()
            // isUpcomming()
            if (isPrevious(meeting))
            {
                //means that the meeting has ended
                return new Error(SystemEnums.ErrorMsg.MeetingInPast.ToDescriptionString());
            }

            if (meeting.MeetingParticipant.Any(x => x.ParticipantId == CurrentUser.Id))
            {

                var MeetingParticipant = meeting.MeetingParticipant.FirstOrDefault(x => x.ParticipantId == CurrentUser.Id);
                MeetingParticipant.Response = true;
                _Context.SaveChanges();
                //Instead of mapping to Dto To include profile and participent profile to be used in email
                var result = (Success)Get(meeting.Id);
                var MeetingDto = result.Data;
                this.SendMeetingAcceptingEmail(MeetingDto);
                try
                {
                    this.CreateNotificationsParticipant(MeetingDto, " accept the invitation for " + MeetingDto.Name + ".", " قبل دعوتك لحضور " + MeetingDto.Name + ".");
                }
                catch (Exception e)
                {

                    ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
                }
                return new Success(true, ResponseMessage.MeetingAccepted.ToDescriptionString());
            }
            return new Error(SystemEnums.ErrorMsg.NotFound.ToDescriptionString());
        }
        public Result DeclineProposal(Guid ID, Guid participant)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.meetingNotFound.ToDescriptionString());
            if (isPrevious(meeting))
                return new Error(SystemEnums.ErrorMsg.MeetingInPast.ToDescriptionString());

            if (meeting.MeetingParticipant.Any(x => x.ParticipantId == CurrentUser.Id))
            {
                var MeetingParticipant = meeting.MeetingParticipant.FirstOrDefault(x => x.ParticipantId == CurrentUser.Id);
                //MeetingParticipant.Response = false;
                //_Context.SaveChanges();
                //Instead of mapping to Dto To include profile and participent profile to be used in email
                var result = (Success)Get(meeting.Id);
                var MeetingDto = result.Data;
                this.SendProposalDeclineEmail(MeetingDto, participant);
                return new Success(true, ResponseMessage.ProposalDecline.ToDescriptionString());
            }
            return new Error(SystemEnums.ErrorMsg.ParticipantNotFound.ToDescriptionString());
        }

        public Result Decline(Guid ID)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.meetingNotFound.ToDescriptionString());
            if (isPrevious(meeting))
                return new Error(SystemEnums.ErrorMsg.MeetingInPast.ToDescriptionString());

            if (meeting.MeetingParticipant.Any(x => x.ParticipantId == CurrentUser.Id))
            {
                var MeetingParticipant = meeting.MeetingParticipant.FirstOrDefault(x => x.ParticipantId == CurrentUser.Id);
                MeetingParticipant.Response = false;
                _Context.SaveChanges();
                //Instead of mapping to Dto To include profile and participent profile to be used in email
                var result = (Success)Get(meeting.Id);
                var MeetingDto = result.Data;
                this.SendMeetingDeclineEmail(MeetingDto);
                try
                {
                    this.CreateNotificationsParticipant(MeetingDto, " reject the invitation for " + MeetingDto.Name + ".", " رفض دعوتك لحضور " + MeetingDto.Name + ".");
                }
                catch (Exception e)
                {

                    ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
                }
                return new Success(true, ResponseMessage.MeetingDecline.ToDescriptionString());
            }
            return new Error(SystemEnums.ErrorMsg.ParticipantNotFound.ToDescriptionString());
        }

        public Result AddNote(Guid ID, string note)
        {
            var meeting = _Context.Meeting.FirstOrDefault(x => x.Id == ID);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.NotFound.ToDescriptionString());
            if (meeting.StartedAt == null)
                return new Error(SystemEnums.ErrorMsg.MeetingNotStartedYet.ToDescriptionString());
            if (meeting.EndedAt != null)
                return new Error(SystemEnums.ErrorMsg.MeetingAlreadyEnded.ToDescriptionString());
            //if (ToTimeZone(meeting.To) < ToTimeZone(DateTime.Now))
            //    return new Error(SystemEnums.ErrorMsg.MeetingInPast.ToDescriptionString());
            meeting.Notes = note;
            _Context.SaveChanges();
            var result = (Success)Get(meeting.Id);
            var MeetingDto = result.Data;
            return new Success(MeetingDto);
        }

        public Result Join(Guid MeetingId)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).FirstOrDefault(x => x.Id == MeetingId);
            if (meeting == null)
                return new Error(SystemEnums.ErrorMsg.meetingNotFound.ToDescriptionString());
            if (meeting.StartedAt == null && ToTimeZone(meeting.From) > ToTimeZone(DateTime.Now))
                return new Error(SystemEnums.ErrorMsg.MeetingNotStartedYet.ToDescriptionString());
            if (meeting.EndedAt != null)
                return new Error(SystemEnums.ErrorMsg.MeetingAlreadyEnded.ToDescriptionString());
            var Participant = _Context.MeetingParticipant.Where(x => x.MeetingId == MeetingId && x.ParticipantId == CurrentUser.Id)
                 .FirstOrDefault();
            if (Participant == null)
                return new Error(ErrorMsg.ParticipantNotFound.ToDescriptionString());
            //if (ToTimeZone(meeting.To) < ToTimeZone(DateTime.Now))
            //    return new Error(SystemEnums.ErrorMsg.MeetingInPast.ToDescriptionString());

            Participant.JoinedMeeting = true;
            Participant.JoinedMeetingTime = DateTime.UtcNow;
            _Context.SaveChanges();
            MeetingDto meetingDto = _mapper.Map<MeetingDto>(meeting);
            try
            {
                this.CreateNotificationsParticipant(meetingDto, " joined the meeting " + meeting.Name + ".", " ألتحق بالاجتماع " + meeting.Name + ".");
            }
            catch (Exception e)
            {

                ElmahCore.ElmahExtensions.RiseError(_ContextAccessor.HttpContext, e);
            }
            return new Success(_mapper.Map<MeetingParticipantDto>(Participant), ResponseMessage.MeetingJoined.ToDescriptionString());
        }

        public Result GetJoinedParticipants(Guid meetingId)
        {
            var JoinedParticipants = _Context.MeetingParticipant
                .Where(x => x.MeetingId == meetingId && x.JoinedMeeting == true)
                .Select(x => new MeetingParticipantDto
                {
                    Id = x.Id,
                    JoinedMeeting = x.JoinedMeeting,
                    JoinedMeetingTime = x.JoinedMeetingTime,
                    MeetingId = x.MeetingId,
                    ParticipantId = x.ParticipantId,
                    Response = x.Response,
                    Participant = _mapper.Map<UserDto>(x.Participant)
                }).AsQueryable();
            if (JoinedParticipants != null)
                return new Success(JoinedParticipants);
            else
                return new Error(ErrorMsg.ServerError.ToDescriptionString());
        }
        #region helpers

        /* private bool IsUniqueMeeting(Guid ID, string Name)
         {
             return !_Context.Meeting.Any(x => x.Id != ID && (x.Name.ToLower().Equals(Name.ToLower())) && (x.IsDeleted == false));
         }
         */
        private bool IsValidMeeting(DateTime from, DateTime to, int topicsSum)
        {
            bool result = false;
            if (from < to)
                if ((to - from).TotalMinutes >= topicsSum)
                    result = true;
            return result;
        }

        private bool IsValidMeeting(DateTime from, DateTime to)
        {
            if (ToTimeZone(DateTime.UtcNow) > ToTimeZone(from))
                return false;
            return true;
        }
        private bool IsValidMeeting(DateTime from, DateTime to, List<MeetingParticipantDto> meetingParticipantDto)
        {
            bool result = true;
            //foreach (var meetingParticipant in meetingParticipantDto)
            //{
            //    meetingParticipant.Participant = _mapper.Map<UserDto>(_Context.User.Where(x => x.Id == meetingParticipant.ParticipantId).FirstOrDefault());
            //    if (meetingParticipant.Participant.VacationFkUser
            //        .Any(x => DateTimeIntervalIntersection(from, to, x.StartDate, x.EndDate)))
            //        result = false;
            //}
            return result;
        }

        private bool DateTimeIntervalIntersection(DateTime MeetingStart, DateTime MeetingEnd, DateTime VacationStart, DateTime VacationEnd)
        {
            //this is a much cleaner and smarter way :D mhussein
            //to check if it overlaps with any meeting 
            //need to make both times utc 

            return MeetingStart < ToTimeZone(VacationEnd) && ToTimeZone(VacationStart) < MeetingEnd;

            //VacationStart = ToTimeZone(VacationStart);
            //VacationEnd = ToTimeZone(VacationEnd);

            //bool Intersect = false;
            //if (MeetingStart == VacationStart || MeetingEnd == VacationEnd)
            //    return true; // If any set is the same time, then by default there must be some overlap. 

            //if (VacationStart < MeetingStart)
            //{
            //    if (VacationEnd > MeetingStart && VacationEnd < MeetingEnd)
            //        return true; // Condition 1

            //    if (VacationEnd > MeetingEnd)
            //        return true; // Condition 3
            //}
            //else
            //{
            //    if (MeetingEnd > VacationStart && MeetingEnd < VacationEnd)
            //        return true; // Condition 2

            //    if (MeetingEnd > VacationEnd)
            //        return true; // Condition 4
            //}


            //return Intersect;
        }
        private Dictionary<string, string> InitializeParams(MeetingDto meetingDto)
        {
            return new Dictionary<string, string>()
            {
                //{ "Name", meetingDto.Name },
                //{ "From", ToTimeZone(meetingDto.From).ToString("dddd mmm yyyy") },
                //{ "To", ToTimeZone(meetingDto.To).ToString("dddd mmm yyyy") }
                {"Name", meetingDto.CreatedByNavigation.FkUserProfile.FirstName },
                {"MeetingTitle", meetingDto.Name},
                {"MeetingParticipant", CurrentUser.Name}
            };
        }
        private List<UserDto> InitializeToList(MeetingDto meetingDto)
        {
            var To = new List<UserDto>();
            //To.Add(new UserDto()
            //{
            //    Email = meetingDto.CreatedByNavigation.Email,
            //    Name = meetingDto.CreatedByNavigation.FkUserProfile.FirstName
            //});
            foreach (var item in meetingDto.MeetingParticipant.Where(mp => !mp.ParticipantId.Equals(CurrentUser.Id)))
            {
                To.Add(new UserDto()
                {
                    Email = item.Participant.Email,
                    Name = item.Participant.FkUserProfile.FirstName,
                    Id = item.Participant.Id

                });
            }
            return To;
        }

        private bool CreateNotifications(MeetingDto meetingDto, string message, string messageAR)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).Include("MeetingParticipant.Participant").Include("MeetingParticipant.Participant.FkUserConfiguration")
                .Include(x => x.CreatedByNavigation.FkUserProfile)
                .FirstOrDefault(x => x.Id == meetingDto.Id);
            foreach (var item in meeting.MeetingParticipant)
            {
                try
                {
                    /*jo invited you to new meeting*/

                    if (!CurrentUser.Id.Equals(item.ParticipantId) && !item.Participant.FkUserConfiguration.NotificationMuted)
                    {
                        _Context.Notification.Add(new Notification()
                        {
                            Message = meeting.CreatedByNavigation.FkUserProfile.FirstName + message,
                            UserId = item.ParticipantId,
                            DateTime = meeting.CreatedAt,
                            MeetingID = meeting.Id,
                            MessageAR = meeting.CreatedByNavigation.FkUserProfile.FirstName + messageAR


                        });
                        _Context.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
        private bool CreateNotificationsParticipant(MeetingDto meetingDto, string message, string messageAR)
        {
            var meeting = _Context.Meeting.Include(x => x.MeetingParticipant).Include("MeetingParticipant.Participant").Include("MeetingParticipant.Participant.FkUserConfiguration")
                .Include(x => x.CreatedByNavigation.FkUserProfile).Include(x=>x.CreatedByNavigation.FkUserConfiguration)
                .FirstOrDefault(x => x.Id == meetingDto.Id);
                try
                {
                    /*jo invited you to new meeting*/

                    if (!meeting.CreatedByNavigation.FkUserConfiguration.NotificationMuted)
                    {
                        _Context.Notification.Add(new Notification()
                        {
                            Message = CurrentUser.Name + message,
                            UserId = meeting.CreatedBy,
                            DateTime = meeting.CreatedAt,
                            MeetingID = meeting.Id,
                            MessageAR = CurrentUser.Name + messageAR


                        });
                        _Context.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    return false;
                }
          
            return true;
        }
        private List<MeetingParticipantDto> GetParticipantsWhoAcceptedMeeting(MeetingDto meetingDto)
        {
            return meetingDto.MeetingParticipant.Where(x => x.Response == true).ToList();
        }
        public Result SendMeetingCreationEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("NewMeetingEmailTemplate"));

            //var Params = InitializeParams(meetingDto);

            var To = InitializeToList(meetingDto);
            foreach (var to in To)
            {

                var acceptMeeting = QueryConstructor("Meeting", "Accept", meetingDto.Id, to.Id, to.IsAdmin);
                var declineMeeting = QueryConstructor("Meeting", "Decline", meetingDto.Id, to.Id, to.IsAdmin);
                var user = _Context.User.Include(u => u.FkUserConfiguration).SingleOrDefault(u => u.Id == to.Id);
                //if ((bool)user.FkUserConfiguration.IntegrationWithGoogleCalendar)
                //    _googleCalender.AddMeeting(meetingDto);
                var Params = new Dictionary<string, string>()
                {
                    {"Name", to.Name},
                    {"CreatedByName", meetingDto.CreatedByNavigation.FkUserProfile.FirstName},
                    {"MeetingStart", meetingDto.From.ToString()},
                    {"MeetingLocation", meetingDto.Location.Name},
                    {"logo", template.Images[0] },
                    {"txt", template.Images[1] },
                    {"soc_1", template.Images[2]},
                    {"soc_2", template.Images[3]},
                    {"soc_3", template.Images[4]},
                    {"soc_4", template.Images[5]},
                    {"acceptMeeting", acceptMeeting },
                    {"declineMeeting", declineMeeting }
                };

                this.SendEmail(new Mail
                {
                    From = new UserDto
                    {
                        Email = _appSettings.EmailSettings.Email,
                        Name = _appSettings.EmailSettings.Email
                    },
                    To = new List<UserDto> { to },
                    Subject = "New Meeting has been created.",
                    Text = "New Meeting has been created.",
                    Html = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", template.Path), Params),
                    Attachments = template.Images
                }, template.Path, Params);
            }
            return new Success("Email Sent.");
        }
        public Result SendMeetingUpdatingEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("UpdatedMeetingEmailTemplate"));

            var To = InitializeToList(meetingDto);

            foreach (var to in To)
            {
                var meetingDetail = QueryConstructor("", "", meetingDto.Id, to.Id, to.IsAdmin);
                var Params = new Dictionary<string, string>()
            {
                {"Name", to.Name},
                {"CreatedByName", meetingDto.CreatedByNavigation.FkUserProfile.FirstName},
                {"MeetingStart", meetingDto.From.ToString()},
                {"MeetingLocation", meetingDto.Location.Name},
                {"logo", template.Images[0] },
                {"txt", template.Images[1] },
                {"soc_1", template.Images[2]},
                {"soc_2", template.Images[3]},
                {"soc_3", template.Images[4]},
                {"soc_4", template.Images[5]},
                {"meetingDetail", meetingDetail}
            };

                this.SendEmail(new Mail
                {
                    From = new UserDto
                    {
                        Email = _appSettings.EmailSettings.Email,
                        Name = _appSettings.EmailSettings.Email
                    },
                    To = new List<UserDto> { to },
                    Subject = "Meeting has been updated.",
                    Text = "Meeting has been updated.",
                    Attachments = template.Images
                }, template.Path, Params);
            }
            return new Success("Email Sent.");
        }
        public Result SendMeetingDeleteEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("DeletedMeetingEmailTemplate"));
            var ParticipantsWhoAccepted = GetParticipantsWhoAcceptedMeeting(meetingDto);
            var To = new List<UserDto>();
            foreach (var item in ParticipantsWhoAccepted)
            {
                To.Add(new UserDto()
                {
                    Email = item.Participant.Email,
                    Name = item.Participant.FkUserProfile.FirstName
                });
            }

            foreach (var to in To)
            {
                var Params = new Dictionary<string, string>()
            {
                {"Name", to.Name},
                {"CreatedByName", meetingDto.CreatedByNavigation.FkUserProfile.FirstName},
                {"MeetingStart", meetingDto.From.ToString()},
                {"MeetingLocation", meetingDto.Location.Name},
                {"logo", template.Images[0] },
                {"txt", template.Images[1] },
                {"soc_1", template.Images[2]},
                {"soc_2", template.Images[3]},
                {"soc_3", template.Images[4]},
                {"soc_4", template.Images[5]}
            };

                this.SendEmail(new Mail
                {
                    From = new UserDto
                    {
                        Email = _appSettings.EmailSettings.Email,
                        Name = _appSettings.EmailSettings.Email
                    },
                    To = new List<UserDto> { to },
                    Subject = "Meeting has been deleted.",
                    Text = "Meeting has been deleted.",
                    Attachments = template.Images
                }, template.Path, Params);
            }
            return new Success("Email Sent.");
        }
        //public Result SendMeetingStatusEmail(MeetingDto meetingDto)
        //{
        //    var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingStatusEmailTemplate"));
        //    var Params = InitializeParams(meetingDto);
        //    var To = InitializeToList(meetingDto);

        //    return this.SendEmail(new Mail
        //    {
        //        From = new UserDto
        //        {
        //            Email = _appSettings.EmailSettings.Email,
        //            Name = _appSettings.EmailSettings.Email
        //        },
        //        To = To,
        //        Subject = "Meeting Status has been changed.",
        //        Text = "Meeting Status has been changed.",
        //        Html = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", template.Path), Params),
        //        Attachments = template.Attachments
        //    }, template.Path, Params);
        //}
        public Result SendMeetingStartedEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingStartedEmailTemplate"));
            var ParticipantsWhoAccepted = GetParticipantsWhoAcceptedMeeting(meetingDto);
            var To = new List<UserDto>();
            foreach (var item in ParticipantsWhoAccepted)
            {
                To.Add(new UserDto()
                {
                    Email = item.Participant.Email,
                    Name = item.Participant.FkUserProfile.FirstName,
                    Id = item.Participant.Id,
                    IsAdmin = item.Participant.IsAdmin
                });
            }

            foreach (var to in To)
            {
                var Params = new Dictionary<string, string>()
            {
                {"Name", to.Name},
                {"CreatedByName", meetingDto.CreatedByNavigation.FkUserProfile.FirstName},
                {"MeetingStart", meetingDto.From.ToString()},
                {"MeetingLocation", meetingDto.Location.Name},
                {"Meetingdetails", meetingDto.Id.ToString()},
                { "proposeNewTime" ,  QueryConstructor("Meeting", "ProposeNewTime", meetingDto.Id, to.Id, to.IsAdmin)},
                { "meetingJoin" ,  QueryConstructor("Meeting", "Join", meetingDto.Id, to.Id, to.IsAdmin)},
                {"meetingDetail", QueryConstructor("", "", meetingDto.Id, to.Id, to.IsAdmin)},
                { "logo", template.Images[0] },
                {"txt", template.Images[1] },
                {"soc_1", template.Images[2]},
                {"soc_2", template.Images[3]},
                {"soc_3", template.Images[4]},
                {"soc_4", template.Images[5]}
            };

                this.SendEmail(new Mail
                {
                    From = new UserDto
                    {
                        Email = _appSettings.EmailSettings.Email,
                        Name = _appSettings.EmailSettings.Email
                    },
                    To = new List<UserDto> { to },
                    Subject = "Meeting has started.",
                    Text = "Meeting has started.",
                    Attachments = template.Images
                }, template.Path, Params);
            }
            return new Success("Email Sent.");
        }
        //public Result SendMeetingPostponedEmail(MeetingDto meetingDto)
        //{
        //    var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingPostponedEmailTemplate"));
        //    var Params = InitializeParams(meetingDto);
        //    var ParticipantsWhoAccepted = GetParticipantsWhoAcceptedMeeting(meetingDto);
        //    var To = new List<UserDto>();
        //    foreach (var item in ParticipantsWhoAccepted)
        //    {
        //        To.Add(new UserDto()
        //        {
        //            Email = item.Participant.Email,
        //            Name = item.Participant.FkUserProfile.FirstName
        //        });
        //    }

        //    return this.SendEmail(new Mail
        //    {
        //        From = new UserDto
        //        {
        //            Email = _appSettings.EmailSettings.Email,
        //            Name = _appSettings.EmailSettings.Email
        //        },
        //        To = To,
        //        Subject = "Meeting is postponed.",
        //        Text = "Meeting is postponed.",
        //        Html = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", template.Path), Params),
        //        Attachments = template.Attachments
        //    }, template.Path, Params);
        //}
        public Result SendMeetingEndedEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingEndedEmailTemplate"));
            // var Params = InitializeParams(meetingDto);
            var ParticipantsWhoAccepted = GetParticipantsWhoAcceptedMeeting(meetingDto);
            var To = new List<UserDto>();
            var reportLink = $"https://localhost:443/ReportServer/?%2fmeetingNote&rs:format=pdf&meeting_id="+meetingDto.Id;
            foreach (var item in ParticipantsWhoAccepted)
            {
                To.Add(new UserDto()
                {
                    Email = item.Participant.Email,
                    Name = item.Participant.FkUserProfile.FirstName
                });
            }
            foreach (var to in To)
            {
                //string meetingReportPDF = $"http://196.202.106.31/ReportServer/?%2fmeetingNote&rs:format=pdf&meeting_id=" + meetingDto.Id;
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.Credentials = new NetworkCredential("karee", "Bugati159");
                    webClient.DownloadFile(reportLink, $"wwwroot/Reports/{meetingDto.Id}.pdf");
                }
                catch(Exception ex)
                {
                    // Path is not a valid path , try changing it
                }
                var Params = new Dictionary<string, string>() 
                { 
                    {"MeetingTitle", meetingDto.Name},
                    {"CreatedByName", meetingDto.CreatedByNavigation.FkUserProfile.FirstName},
                    {"MeetingStart", meetingDto.From.ToString()},
                    {"MeetingLocation", meetingDto.Location.Name},
                    {"MeetingNotes", meetingDto.Notes }
                    // {"Tasks", meetingDto.MeetingTask}
                };
                this.SendEmail(new Mail
                {
                    From = new UserDto
                    {
                        Email = _appSettings.EmailSettings.Email,
                        Name = _appSettings.EmailSettings.Email
                    },
                    To = new List<UserDto> { to },
                    Subject = "Meeting has ended.",
                    Text = "Meeting has ended.",
                    Html = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", template.Path), Params),
                    Attachments = new List<string>() { $"wwwroot/Reports/{meetingDto.Id}.pdf" }
                }, template.Path, Params);
            }
            File.Delete($"wwwroot/Reports/{meetingDto.Id}.pdf");
            return new Success("Email Sent.");

        }
        public Result SendMeetingAcceptingEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("AcceptedMeetingEmailTemplate"));

            var To = new List<UserDto>();
            To.Add(new UserDto()
            {
                Email = meetingDto.CreatedByNavigation.Email,
                Name = meetingDto.CreatedByNavigation.FkUserProfile.FirstName,
                Id = meetingDto.CreatedByNavigation.Id,
                IsAdmin = meetingDto.CreatedByNavigation.IsAdmin
            });
            var meetingDetail = QueryConstructor("", "", meetingDto.Id, To.First().Id, To.First().IsAdmin);
            var Params = new Dictionary<string, string>()
            {
                {"Name", meetingDto.CreatedByNavigation.FkUserProfile.FirstName },
                {"MeetingTitle", meetingDto.Name},
                {"MeetingParticipant", CurrentUser.Name},
                {"logo", template.Images[0]},
                {"txt", template.Images[1]},
                {"soc_1", template.Images[2]},
                {"soc_2", template.Images[3]},
                {"soc_3", template.Images[4]},
                {"soc_4", template.Images[5]},
                {"meetingDetail",meetingDetail}
            };
            return this.SendEmail(new Mail
            {
                From = new UserDto
                {
                    Email = _appSettings.EmailSettings.Email,
                    Name = _appSettings.EmailSettings.Email
                },
                To = To,
                Subject = "Participent has accepted the Meeting.",
                Text = "Participent has accepted the Meeting.",
                Attachments = template.Images
            }, template.Path, Params);
        }
        public Result SendMeetingDeclineEmail(MeetingDto meetingDto)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("DeclinedMeetingEmailTemplate"));

            var To = new List<UserDto>();
            To.Add(new UserDto()
            {
                Email = meetingDto.CreatedByNavigation.Email,
                Name = meetingDto.CreatedByNavigation.FkUserProfile.FirstName,
                Id = meetingDto.CreatedByNavigation.Id,
                IsAdmin = meetingDto.CreatedByNavigation.IsAdmin
            });

            var Params = new Dictionary<string, string>()
            {
                {"Name", meetingDto.CreatedByNavigation.FkUserProfile.FirstName },
                {"MeetingTitle", meetingDto.Name},
                {"meetinDetail", QueryConstructor("","",meetingDto.Id,To.First().Id,To.First().IsAdmin) },
                {"MeetingParticipant", CurrentUser.Name},
                {"logo", template.Images[0]},
                {"txt", template.Images[1]},
                {"soc_1", template.Images[2]},
                {"soc_2", template.Images[3]},
                {"soc_3", template.Images[4]},
                {"soc_4", template.Images[5]}
            };

            return this.SendEmail(new Mail
            {
                From = new UserDto
                {
                    Email = _appSettings.EmailSettings.Email,
                    Name = _appSettings.EmailSettings.Email
                },
                To = To,
                Subject = "Participent has declined the Meeting.",
                Text = "Participent has declined the Meeting.",
                Attachments = template.Images
            }, template.Path, Params);
        }

        public Result SendProposalDeclineEmail(MeetingDto meetingDto, Guid participant)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("DeclinedproposalEmailTemplate"));
            var ToUser = _Context.User.Where(x => x.Id == participant).Include(x => x.FkUserProfile).FirstOrDefault();
            if (ToUser != null)
            {
                var To = new List<UserDto>();
                To.Add(new UserDto()
                {
                    Email = ToUser.Email,
                    Name = ToUser.FkUserProfile.FirstName,
                    Id = ToUser.Id,
                    IsAdmin = ToUser.IsAdmin
                });

                var Params = new Dictionary<string, string>()
            {
                {"Name", meetingDto.CreatedByNavigation.FkUserProfile.FirstName },
                {"MeetingTitle", meetingDto.Name},
                {"meetingDetail", QueryConstructor("","",meetingDto.Id,To.First().Id,To.First().IsAdmin) },
                {"MeetingParticipant", CurrentUser.Name},
                {"logo", template.Images[0]},
                {"txt", template.Images[1]},
                {"soc_1", template.Images[2]},
                {"soc_2", template.Images[3]},
                {"soc_3", template.Images[4]},
                {"soc_4", template.Images[5]}
            };

                return this.SendEmail(new Mail
                {
                    From = new UserDto
                    {
                        Email = _appSettings.EmailSettings.Email,
                        Name = _appSettings.EmailSettings.Email
                    },
                    To = To,
                    Subject = "Creator has declined the Proposal.",
                    Text = "Creator has declined the Proposal.",
                    Attachments = template.Images
                }, template.Path, Params);
            }
            return null;
        }

        private bool isPrevious(Meeting meeting)
        {
            return meeting.EndedAt != null || (ToTimeZone(meeting.To) < ToTimeZone(DateTime.UtcNow) && meeting.StartedAt == null);
        }

        private bool isCurrent(Meeting meeting)
        {
            return ToTimeZone(meeting.From) <= ToTimeZone(DateTime.UtcNow)
                || ToTimeZone(meeting.To) >= ToTimeZone(DateTime.UtcNow)
                || (meeting.StartedAt != null && (ToTimeZone((DateTime)meeting.StartedAt) <= ToTimeZone(DateTime.UtcNow)));
        }

        private bool isUpComming(Meeting meeting)
        {
            return meeting.StartedAt != null && ToTimeZone(meeting.From) > ToTimeZone(DateTime.UtcNow);
        }

        public Result PurposeNewTime(Guid ID, DateTime PurposedTo)
        {
            //get the meeting
            Meeting meeting = _Context.Meeting.Where(m => m.Id.Equals(ID)).SingleOrDefault();
            if (meeting == null)
            {
                //meeting is not found;
                return new Error(ErrorMsg.NotFound.ToDescriptionString());
            }
            else
            {
                //we found the meeting
                //check if the date is a valid date
                if (PurposedTo < DateTime.Now)
                {
                    return new Error(ErrorMsg.dateInThePast.ToDescriptionString());
                }
                else
                {
                    //all is ok send the creator id
                    //getting the email and name of the creator only to increase the speed of the query
                    var meetingCreator = _Context.User.Where(u => u.Id.Equals(meeting.CreatedBy)).Select(x => new { x.Email, x.Name, x.Id, x.IsAdmin }).SingleOrDefault();
                    UserDto emailSender = new UserDto
                    {
                        Email = CurrentUser.Email,
                        Name = CurrentUser.Name
                    };
                    List<UserDto> emailRecipients = new List<UserDto>();
                    emailRecipients.Add(new UserDto { Email = meetingCreator.Email, Name = meetingCreator.Name });
                    var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingTimeChangePurpose"));

                    //redirect/token/module/action/meetingId

                    string AcceptLink = QueryConstructor("Meeting", "edit", meeting.Id, meetingCreator.Id, meetingCreator.IsAdmin);// + "&proposedTime=" + PurposedTo;
                    string RejectLink = QueryConstructor("Meeting", "DeclineProposal", meeting.Id, meetingCreator.Id, meetingCreator.IsAdmin) + "&participant=" + CurrentUser.Id;
                    Dictionary<string, string> _params = new Dictionary<string, string>()
                    {
                        {"Reciver",meetingCreator.Name},
                        {"PurposedBy", CurrentUser.Name },
                        {"MeetingName", meeting.Name },
                        {"AcceptLink", AcceptLink },
                        {"RejectLink", RejectLink },
                        {"ProposedFrom", PurposedTo.ToString() } ,
                        { "logo", template.Images[0]},
                        {"txt", template.Images[1]},
                        {"soc_1", template.Images[2]},
                        {"soc_2", template.Images[3]},
                        {"soc_3", template.Images[4]},
                        {"soc_4", template.Images[5]}
                    };
                    this.SendEmail(new Mail
                    {
                        From = emailSender,
                        To = emailRecipients,
                        Subject = "proposal of a new meeting time",
                        Text = string.Format("{0} participant of meeting: {1} is proposing to change the meeting time to start from {2}", CurrentUser.Name, meeting.Name, PurposedTo),
                        Html = this.PopulateBody(string.Concat(this._env.WebRootPath, "/", template.Path), _params),
                        Attachments = template.Images
                    }, template.Path, _params);

                    return new Success("meeting purposed successfully");
                }
            }
        }

        public Result AcceptProposal(Guid meetingId, DateTime proposedTime)
        {
            MeetingDto meetingDto = Get(meetingId).Data;
            foreach (var item in meetingDto.MeetingTag)
            {
                item.Id = new Guid();
                item.MeetingId = new Guid();
            }
            //check if the meeting is present
            if (meetingDto == null)
            {
                //meeting not found
                return new Error(ErrorMsg.meetingNotFound.ToDescriptionString());
            }
            meetingDto.From = proposedTime;
            return Update(meetingDto);
        }

        private string GenerateToken(Guid id)
        {
            //UserDto tokenObj;
            //if (systemConfiguration.AuthenticationMode == (int)AuthenticateMode.ActiveDirctory)
            //    tokenObj = _accountService.Authenticate(CurrentUser.Name, Shared.Security.TripleDES.Decrypt(CurrentUser.Password, true)).Data;
            //else tokenObj = _accountService.Authenticate(CurrentUser.Email, Shared.Security.TripleDES.Decrypt(CurrentUser.Password, true)).Data;


            UserDto tokenObj = _accountService.Authenticate("mody199513@gmail.com", "12345678", DateTime.Now, true).Data;///Ask Hala for this 
            var token = tokenObj.Token;
            return $"{_currentBaseUrl}/Meetings/anonymous/view?id={id}&access_token={token}";
        }

        private string QueryConstructor(string module, string action, Guid meetingId, Guid userId, bool isAdmin)
        {
            var _token = _accountService.GenerateToken(new UserDto { Id = userId, IsAdmin = isAdmin });
            var TokenHandler = new JwtSecurityTokenHandler();
            //token/module/action/meetingid
            if (action == "Join") return _currentBaseUrl + string.Format(@"Redirect?access_token={0}&module={1}&action={2}&MeetingId={3}&Participant={4}", TokenHandler.WriteToken(_token), module, action, meetingId, userId);
            else return _currentBaseUrl + string.Format(@"Redirect?access_token={0}&module={1}&action={2}&ID={3}&Participant={4}", TokenHandler.WriteToken(_token), module, action, meetingId, userId);


        }
        //public List<DateTime> GetWeekDates()
        //{
        //    DateTime today = DateTime.Now;
        //    List<DateTime> ListofWeekDates = new List<DateTime>();
        //    var weekday = (int)today.DayOfWeek;
        //    for (int i=0;i<=4;i++)
        //    {
        //        var x = today;
        //        x= x.AddDays(i- weekday);
        //        ListofWeekDates.Add(x);
        //    }
        //    return ListofWeekDates;
        //}
        //public List<Meeting> GetAllWeekMeetings(DateTime weekStart,DateTime weekEnd)
        //{
        //    return _Context.Meeting.Where(x => x.From >= weekStart && x.To <= weekEnd).ToList();
        //}
        //public List<MeetingDto> GetPreviousMeetings(DateTime DayDate,List<MeetingDto> WeekMeetings)
        //{
        //    return WeekMeetings.Where(x => x.From.Day == DayDate.Day &&
        //    (x.To < DayDate || x.Started == true && x.NotEnded == false)).ToList();

        //}

        //public List<MeetingDto> GetUpcomingMeetings(DateTime DayDate, List<MeetingDto> WeekMeetings)
        //{
        //    return WeekMeetings.Where(x => x.From.Day == DayDate.Day &&
        //       x.From > DayDate).ToList();
        //}

        //public Result GetCurrentMeetings(DateTime DayDate)
        //{
        //    var Meetings = _Context.Meeting
        //        .Where(x => x.IsDeleted == false && (x.StartedAt != null && x.EndedAt == null)
        //        && ToTimeZone(x.From) <= ToTimeZone(DateTime.Now) &&
        //       ToTimeZone(DateTime.Now) <= ToTimeZone(x.To))
        //        .Select(x => new MeetingDto
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            LocationId = x.LocationId,
        //            ProjectId = x.ProjectId,
        //            From = ToTimeZone(x.From),
        //            To = ToTimeZone(x.To),
        //            Notes = x.Notes,
        //            StartedAt = x.StartedAt,
        //            EndedAt = x.EndedAt,
        //            CreatedBy = x.CreatedBy,
        //            CreatedAt = x.CreatedAt,
        //            UpdatedAt = x.UpdatedAt,
        //            UpdatedBy = x.UpdatedBy,
        //            MeetingTopic = _mapper.Map<ICollection<MeetingTopicDto>>(x.MeetingTopic.Where(y => y.IsDeleted != true)
        //            .Select(y => new MeetingTopicDto
        //            {
        //                Id = y.Id,
        //                Name = y.Name,
        //                Duration = y.Duration,
        //                PresenterId = y.PresenterId,
        //                CreatedAt = y.CreatedAt,
        //                CreatedBy = y.CreatedBy,
        //                Presenter = new UserDto
        //                {
        //                    Name = y.Presenter.Name
        //                }

        //            })
        //            ),
        //            MeetingTag = _mapper.Map<ICollection<MeetingTagDto>>(x.MeetingTag),
        //            MeetingTask = _mapper.Map<ICollection<MeetingTaskDto>>(x.MeetingTask),
        //            Location = _mapper.Map<LocationDto>(x.Location),
        //            CreatedByNavigation = new UserDto
        //            {
        //                Email = x.CreatedByNavigation.Email,
        //                FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile)
        //            },
        //            MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
        //                x.MeetingParticipant
        //           .Select(y => new MeetingParticipantDto
        //           {
        //               Id = y.Id,
        //               MeetingId = y.MeetingId,
        //               ParticipantId = y.ParticipantId,
        //               Response = y.Response,
        //               JoinedMeeting = y.JoinedMeeting,
        //               JoinedMeetingTime = y.JoinedMeetingTime,
        //               Participant = new UserDto
        //               {
        //                   Id = y.Participant.Id,
        //                   Name = y.Participant.Name,
        //                   Email = y.Participant.Email,
        //                   FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile)
        //               }
        //           }))
        //        })
        //        .AsQueryable();
        //    if (Meetings != null)
        //        return new Success(dataSource.ToResult(Meetings));
        //    else
        //        return new Error(ErrorMsg.ServerError.ToDescriptionString());
        //}
        #endregion
    }
}
