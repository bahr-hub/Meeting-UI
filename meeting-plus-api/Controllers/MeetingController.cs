using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MeetingPlus.API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Model.DTO;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiController]
    public class MeetingController : BaseController
    {
        IMeetingService _MeetingService;
        private IHubContext<MeetingHub> _hub;
        public MeetingController(IMeetingService MeetingService, IHubContext<MeetingHub> hubContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _MeetingService = MeetingService;
            _hub = hubContext;

        }

        /// <summary>
        /// Get a meeting by Id
        /// </summary>
        /// <param name="ID">Meeting ID</param>
        [HttpGet()]
        public Result Get(Guid ID)
        {
            return _MeetingService.Get(ID);
        }



        /// <summary>
        /// Get All meetings
        /// </summary>
        [HttpGet]
        public Result GetAll([FromQuery]DataSource dataSource)
        {
            return _MeetingService.GetAll(dataSource);
        }

        [HttpGet]
        public Result GetAllMeetingLite()
        {
            return _MeetingService.GetAllLite();
        }

        [HttpPost]
        public Result GetFiltredMeeting([FromBody]DataSource dataSource)
        {
            return _MeetingService.GetFiltredMeeting(dataSource);
        }

        /// <summary>
        /// Get All meetings in interval
        /// </summary>
        [HttpGet]
        public Result GetInterval(DateTime start, DateTime end)
        {
            return _MeetingService.GetAll(start, end);
        }

        /// <summary>
        /// Creates a meeting
        /// </summary>
        /// <param name="Meeting">Meeting</param>
        /// <remarks>
        /// Sample request:
        ///    
        ///  {
        ///  "name": "Meeting 1",
        ///  "locationId": 42,
        ///  "projectId": 22,
        ///  "status": 0,
        ///  "from": "2019-07-21T11:30:00Z",
        ///  "to": "2019-07-21T14:30:00Z",
        ///  "meetingTopic": [
        ///    {
        ///      "name": "Topic 1",
        ///      "presenterId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///      "duration": 2
        ///    }
        ///  ],
        ///  "meetingParticipant": [
        ///    {
        ///      "participantId": "e004b21b-d340-45de-a3e1-3a2cf248cee8"
        ///    }
        ///   ]
        ///}
        ///
        /// </remarks>
        [HttpPost]
        public Result Create(MeetingDto Meeting)
        {
            Result result = _MeetingService.Create(Meeting);
            //foreach (var part in (result.Data as MeetingDto).MeetingParticipant) 
            //_hub.Clients.User(part.ParticipantId.ToString()).SendAsync("createMeeting", result);
            _hub.Clients.All.SendAsync("createMeeting", result);
            return result;
        }

        /// <summary>
        /// Updates a meeting
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     
        ///   {
        ///  "id": "09b572e9-db12-4a7c-ae87-9621fa41c09b",
        ///  "name": "Meeting 1",
        ///  "locationId": 42,
        ///  "projectId": 22,
        ///  "status": 0,
        ///  "from": "2019-08-01T09:30:00Z",
        ///  "to": "2019-08-01T12:30:00Z",
        ///  "meetingTopic": [
        ///     {
        ///      "id": "9cd90fa9-5b40-476c-bc4c-9f3af7a597ba", 
        ///      "name": "Topic 1 updated",
        ///      "presenterId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///      "duration": 60,
        ///      "createdBy": "08b1f426-6f1a-4b69-a074-6f5a8bcdc4dc",
        ///      "createdAt": "2019-07-30T12:03:39.2444302"
        ///    },
        ///     {
        ///      "name": "Topic 2",    
        ///      "presenterId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///      "duration": 60
        ///    }
        ///  ],
        ///  "meetingParticipant": [
        ///    {
        ///      "id": "e3af153f-0238-423b-94c8-d84a6d2dd2f4",   
        ///      "participantId": "08b1f426-6f1a-4b69-a074-6f5a8bcdc4dc"
        ///    },
        ///     {       
        ///       "participantId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///      }
        ///  ]
        ///}
        ///
        /// </remarks>
        [HttpPost]
        public Result Update(MeetingDto Meeting)
        {
            Result result = _MeetingService.Update(Meeting);
            //foreach (var part in (result.Data as MeetingDto).MeetingParticipant) 
            //_hub.Clients.User(part.ParticipantId.ToString()).SendAsync("createMeeting", result);
            _hub.Clients.All.SendAsync("updateMeeting", result);
            return result;
        }

        /// <summary>
        /// Deletes meeting by id.
        /// </summary>
        /// <param name="id">Meeting ID</param>
        [HttpDelete]
        public Result DeleteMeeting(Guid id)
        {
            Result result = _MeetingService.Delete(id);
            _hub.Clients.All.SendAsync("deleteMeeting", result);
            return result;
        }

        /// <summary>
        /// Gets a list of users that don't have vacation during the meeting interval
        /// </summary>
        /// <param name="MeetingStart">Meeting Start Date and time</param>
        /// /// <param name="MeetingEnd">Meeting End Date and time</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     
        ///     {
        ///        "MeetingStart": 
        ///        "MeetingEnd": 
        ///        
        ///     }
        /// </remarks>
        [HttpPost]
        public Result GetParticipants(DateTime MeetingStart, DateTime MeetingEnd, Guid? MeetingId)
        {
            return _MeetingService.GetParticipants(MeetingStart, MeetingEnd, MeetingId);
        }

        [HttpPost]
        public Result GetLocations(DateTime MeetingStart, DateTime MeetingEnd, Guid? MeetingId)
        {
            return _MeetingService.GetLocations(MeetingStart, MeetingEnd, MeetingId);
        }

        [HttpPost]
        public Result ProposeNewTime(Guid id, DateTime PurposedTo)
        {
            return _MeetingService.PurposeNewTime(id, PurposedTo);
        }
        public Result AcceptProposal(Guid id, DateTime proposedTime)
        {
            return _MeetingService.AcceptProposal(id, proposedTime);
        }
        [HttpGet()]
        public Result GetJoinedParticipants(Guid meetingId)
        {
            return _MeetingService.GetJoinedParticipants(meetingId);
        }
        [HttpPost]
        public Result GetCurrentMeetings([FromBody]DataSource dataSource)
        {
            return _MeetingService.GetCurrentMeetings(dataSource);
        }

        [HttpPost]
        public Result GetPreviousMeetings([FromBody]DataSource dataSource)
        {
            return _MeetingService.GetPreviousMeetings(dataSource);
        }

        [HttpPost]
        public Result GetUpcomingMeetings([FromBody]DataSource dataSource)
        {
            return _MeetingService.GetUpcomingMeetings(dataSource);
        }

        [HttpPost]
        public Result GetAllMeetings([FromBody]DataSource dataSource, [FromQuery] bool isGatting )
        {

            return _MeetingService.GetAllMeetings(dataSource, isGatting);
        }

        [HttpPost]
        public Result Start(Guid ID)
        {
            Result result = _MeetingService.Start(ID);
            _hub.Clients.All.SendAsync("startMeeting", result);
            return result;
            //return _MeetingService.Start(ID);
        }

        public Result Postpone(Guid ID, DateTime PostponedTo)
        {
            Result result = _MeetingService.Postpone(ID, PostponedTo);
            _hub.Clients.All.SendAsync("postponeMeeting", result);
            return result;
            //return _MeetingService.Start(ID);
        }

        [HttpPost]
        public Result End(Guid ID)
        {
            Result result = _MeetingService.End(ID);
            _hub.Clients.All.SendAsync("endMeeting", result);
            return result;
        }
        [HttpPost]
        public Result Join(Guid MeetingId)
        {
            Result result = _MeetingService.Join(MeetingId);
            _hub.Clients.All.SendAsync("joinMeeting", result);
            return result;
        }
        [HttpPost]
        public Result Accept(Guid ID)
        {
            return _MeetingService.Accept(ID);
        }

        [HttpPost]
        public Result Decline(Guid ID)
        {
            return _MeetingService.Decline(ID);
        }
        [HttpPost]
        public Result DeclineProposal(Guid ID,Guid participant)
        {
            return _MeetingService.DeclineProposal(ID, participant);
        }
        [HttpPost]
        public Result AddNote(NoteDto note)
        {
            Result result = _MeetingService.AddNote(note.MeetingID, note.Description);
            _hub.Clients.All.SendAsync("addNote", result);
            return result;
        }
        [HttpPost]
        public Result AddParticipants(Guid ID, List<MeetingParticipantDto> participantDtos)
        {
            Result result = _MeetingService.AddParticipants(ID, participantDtos);
            _hub.Clients.All.SendAsync("addParticipant", result);
            return result;
        }
        [HttpPost]
        public Result DeleteParticipant(Guid ID, Guid MeetingID)
        {
            Result result = _MeetingService.DeleteParticipant(ID, MeetingID);
            _hub.Clients.All.SendAsync("deleteParticipant", result);
            return result;
        }
        //public void GetWeekDates()
        //{
        //    _MeetingService.GetWeekDates();
        //}
        //public Result TestSendEmail()
        //{
        //    var list = new List<MeetingParticipantDto>();

        //    list.Add(new MeetingParticipantDto()
        //    {
        //        Participant = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Raghda" },
        //            Email = "raghdaahmed95@gmail.com"
        //        }
        //    });
        //    var res = _MeetingService.SendMeetingCreationEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });

        //    res = _MeetingService.SendMeetingUpdatingEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });

        //    res = _MeetingService.SendMeetingDeleteEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });

        //    res = _MeetingService.SendMeetingDeclineEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });

        //    res = _MeetingService.SendMeetingStatusEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });
        //    res = _MeetingService.SendMeetingStartedEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });

        //    res = _MeetingService.SendMeetingEndedEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });


        //    res = _MeetingService.SendMeetingAcceptingEmail(new MeetingDto
        //    {
        //        Name = "Test Meeting",
        //        From = DateTime.Now,
        //        To = DateTime.Now,
        //        MeetingParticipant = list,
        //        CreatedByNavigation = new UserDto()
        //        {
        //            FkUserProfile = new UserProfileDto() { FirstName = "Alhassan" },
        //            Email = "AlhassanNageh@gmail.com"
        //        }
        //    });

        //    return res;
        //}
    }
}