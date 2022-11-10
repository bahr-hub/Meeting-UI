using System;
using System.Linq;
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
    public class MeetingTopicController : BaseController
    {
        IMeetingTopicService _MeetingTopicService;
        private IHubContext<MeetingHub> _hub;

        public MeetingTopicController(IMeetingTopicService MeetingTopicService, IHubContext<MeetingHub> hubContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _MeetingTopicService = MeetingTopicService;
            _hub = hubContext;
        }

        /// <summary>
        /// Gets a topic by Id
        /// </summary>
        /// <param name="ID">Meeting topic ID</param>
        [HttpGet()]
        public Result Get(Guid ID)
        {
            return _MeetingTopicService.Get(ID);
        }

        [HttpGet()]
        public Result GetAll([FromQuery]DataSource dataSource)
        {
            return _MeetingTopicService.GetAll(dataSource);
        }

        /// <summary>
        /// Creates a new meeting topic when the meeting has already started and not ended
        /// </summary>
        /// <param name="MeetingTopic">Meeting Topic Object</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///{
        ///  "name": "Topic 123",
        ///  "presenterId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///  "duration": 10,
        ///  "fkMeetingId": "14114bca-8a1c-447e-8f58-bb5896705ca2"
        ///}
        /// </remarks>
        [HttpPost]
        public Result Create([FromBody]MeetingTopicDto MeetingTopic)
        {
            Result result = _MeetingTopicService.Create(MeetingTopic);
            _hub.Clients.All.SendAsync("addTopic", result);
            return result;
        }

        /// <summary>
        /// Updates a meeting topic
        /// </summary>
        /// <param name="MeetingTopic">Meeting topic Object</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///       {
        ///   "id": "9c68d2d9-4bf8-49c9-942d-d76c83a19b18",
        ///   "name": "Topic 123 updated",
        ///   "presenterId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///   "duration": 10,
        ///   "fkMeetingId": "14114bca-8a1c-447e-8f58-bb5896705ca2",
        ///   "isDeleted": false,
        ///     "createdBy": "08b1f426-6f1a-4b69-a074-6f5a8bcdc4dc",
        ///     "createdAt": "2019-07-31T13:33:24.6594879"
        ///}
        /// </remarks>
        [HttpPut]
        public Result Update(MeetingTopicDto MeetingTopic)
        {
            return _MeetingTopicService.Update(MeetingTopic);
        }
        [HttpPost]
        public Result CloseTopic(Guid ID)
        {
            Result result = _MeetingTopicService.CloseTopic(ID);
            _hub.Clients.All.SendAsync("closeTopic", result);
            return result;
        }

        /// <summary>
        /// Deletes a meeting topic
        /// </summary>
        /// <param name="id">Meeting topic id</param>
        [HttpDelete]
        public Result DeleteMeetingTopic(Guid id)
        {
            return _MeetingTopicService.Delete(id);
        }

    }
}