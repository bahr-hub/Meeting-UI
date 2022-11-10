using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;
using Newtonsoft.Json;

namespace Services
{
    public interface IGoogleCalender : IBaseService
    {
        UserCredential Integrate(Guid UserID);
        UserCredential Integrate2();
        void AddMeeting(MeetingDto meeting);
    }
    public class GoogleCalenderService : BaseService, IGoogleCalender
    {
        IAccountService _accountService;
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "meeting plus google calender integration";
        public GoogleCalenderService( IOptions<AppSettings> AppSettings, IAccountService accountService, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env) : base(AppSettings, Mapper, Context, contextAccessor, env)
        {
            _accountService = accountService;
        }

        public void AddMeeting(MeetingDto meeting)
        {
            Event calenderMeeting = new Event();
            //calenderMeeting.Id = meeting.Id.ToString();
            calenderMeeting.Start = new EventDateTime();
            calenderMeeting.End = new EventDateTime();
            calenderMeeting.Start.DateTime = meeting.From;
            calenderMeeting.End.DateTime = meeting.To;
            calenderMeeting.Summary = meeting.Name;
            calenderMeeting.Attendees = new List<EventAttendee>();
            foreach (var participant in meeting.MeetingParticipant)
            {
                if (File.Exists($"wwwroot/googleCalender.json/{participant.Participant.Id}/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user"))
                {
                    calenderMeeting.Attendees.Add(new EventAttendee { DisplayName = participant.Participant.Name, Email = participant.Participant.Email });
                    var meetingCreator = _Context.User.SingleOrDefault(u => u.Id.Equals(meeting.CreatedBy));
                    calenderMeeting.Attendees.Add(new EventAttendee { DisplayName = meetingCreator.Name, Email = meetingCreator.Email, Organizer = true });
                    var service = new CalendarService(new BaseClientService.Initializer()
                    {

                        HttpClientInitializer = this.Integrate(participant.Participant.Id),
                        ApplicationName = ApplicationName,
                    });
                    EventsResource.InsertRequest request = service.Events.Insert(calenderMeeting, "primary");
                    request.Execute();
                }
            }

               
        }

        public UserCredential Integrate(Guid UserID)
        {
            try
            {
           
                UserCredential credential;

                using (var stream =
                new FileStream("wwwroot/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = $"wwwroot/googleCalender.json/{UserID}";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;

                }
                return credential;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public UserCredential Integrate2()
        {
            try
            {
               
                UserCredential credential;

                using (var stream =
                new FileStream("wwwroot/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = $"wwwroot/googleCalender.json/{CurrentUser.Id}";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;

                }
                return credential;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
