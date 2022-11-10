using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using static Shared.SystemEnums;

namespace Services
{
    public interface IMeetingReminderService : IBaseService
    {
        void GetParticipants();
    }
    public class MeetingReminderService : BaseService, IMeetingReminderService
    {
        IAccountService _accountService;
        public MeetingReminderService(IOptions<AppSettings> AppSettings, IAccountService accountService, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { _accountService = accountService; }

        public void GetParticipants()
        {

            DateTime _now = DateTime.UtcNow;
            var NotStartedMeetings = _Context.Meeting
                .Include(x => x.MeetingParticipant)
                .Where(x => x.IsDeleted != true && x.StartedAt == null && x.From > _now)
               .Select(x => new MeetingDto
               {
                   Id = x.Id,
                   From = ToTimeZone(x.From),
                   Location = _mapper.Map<LocationDto>(x.Location),
                   CreatedByNavigation = new UserDto
                   {
                       Email = x.CreatedByNavigation.Email,
                       FkUserProfile = _mapper.Map<UserProfileDto>(x.CreatedByNavigation.FkUserProfile),
                       FkUserConfiguration = _mapper.Map<UserConfigurationDto>(x.CreatedByNavigation.FkUserConfiguration)
                   },
                   MeetingParticipant = _mapper.Map<ICollection<MeetingParticipantDto>>(
                        x.MeetingParticipant
                   .Select(y => new MeetingParticipantDto
                   {
                       Id = y.Id,
                       MeetingId = y.MeetingId,
                       ParticipantId = y.ParticipantId,
                       Response = y.Response,
                       IsReminded = y.IsReminded,
                       Participant = new UserDto
                       {
                           Email = y.Participant.Email,
                           FkUserProfile = _mapper.Map<UserProfileDto>(y.Participant.FkUserProfile),
                           FkUserConfiguration = _mapper.Map<UserConfigurationDto>(y.Participant.FkUserConfiguration)
                       }
                   }))
               })
                .ToList();
            foreach (var meeting in NotStartedMeetings)
            {
                foreach (var participant in meeting.MeetingParticipant)
                {

                    if (participant.Participant.FkUserConfiguration != null
                        && participant.Participant.FkUserConfiguration.ReminderBeforeMeeting != null
                        && participant.IsReminded != true
                        && (meeting.From - _now).TotalMinutes <=
                        participant.Participant.FkUserConfiguration.ReminderBeforeMeeting)
                    {
                        var _date = ToTimeZone(meeting.From);
                        SendMeetingReminderEmail(participant.Participant.FkUserProfile.FirstName, meeting.CreatedByNavigation.FkUserProfile.FirstName
                            , _date, meeting.Location.Name, participant.Participant, meeting.Id);
                        UpdateIsReminded(participant.Id);

                    }
                }
                //if (meeting.CreatedByNavigation.FkUserConfiguration != null 
                //    && meeting.CreatedByNavigation.FkUserConfiguration.ReminderBeforeMeeting!=null
                //        && (ToTimeZone(meeting.From) - ToTimeZone(DateTime.Now)).TotalMinutes <=
                //        meeting.CreatedByNavigation.FkUserConfiguration.ReminderBeforeMeeting)
                //{
                //    SendMeetingReminderEmail(meeting.CreatedByNavigation.FkUserProfile.FirstName, meeting.CreatedByNavigation.FkUserProfile.FirstName
                //        , meeting.From, meeting.Location.Name, meeting.CreatedByNavigation);

                //}

                //if (meeting.Id.ToString()== "dc7caa91-2b3d-4898-9ca2-10c249d7ad32")
                //{
                //    Console.WriteLine(meeting.Id.ToString());
                //    Console.WriteLine(ToTimeZone(meeting.From).ToString());
                //    Console.WriteLine(ToTimeZone(DateTime.Now).ToString());
                //    Console.WriteLine((ToTimeZone(meeting.From) - ToTimeZone(DateTime.Now)).TotalMinutes.ToString());
                //}
            }
        }
        #region helpers
        private string QueryConstructor(string module, string action, Guid meetingId, Guid userId, bool isAdmin)
        {
            var _token = _accountService.GenerateToken(new UserDto { Id = userId, IsAdmin = isAdmin });
            var TokenHandler = new JwtSecurityTokenHandler();
            //token/module/action/meetingid
            if (action == "Join") return _currentBaseUrl + string.Format(@"Redirect?access_token={0}&module={1}&action={2}&MeetingId={3}&Participant={4}", TokenHandler.WriteToken(_token), module, action, meetingId, userId);
            else return _currentBaseUrl + string.Format(@"Redirect?access_token={0}&module={1}&action={2}&ID={3}&Participant={4}", TokenHandler.WriteToken(_token), module, action, meetingId, userId);

            //     return "";
        }
        public Result SendMeetingReminderEmail(string Name, string Creator, DateTime From, string Location, UserDto to, Guid meetingId)
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("MeetingReminderEmailTemplate"));
            var meetingDetail = QueryConstructor("", "", meetingId, to.Id, to.IsAdmin);

            var Params = new Dictionary<string, string>()
            {
                {"Name", Name},
                {"CreatedByName", Creator},
                {"MeetingStart", From.ToString()},
                {"MeetingLocation", Location},
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
                Subject = "Meeting will start soon.",
                Text = "Meeting will start soon.",
                Attachments = template.Images
            }, template.Path, Params);

            return new Success("Email Sent.");
        }
        public void UpdateIsReminded(Guid ParticipantId)
        {
            var participant = _Context.MeetingParticipant.FirstOrDefault(x => x.Id == ParticipantId);
            participant.IsReminded = true;
            _Context.SaveChanges();
        }
        #endregion
    }
}
