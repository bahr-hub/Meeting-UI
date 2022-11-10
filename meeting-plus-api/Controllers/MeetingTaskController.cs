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
    public class MeetingTaskController : BaseController
    {
        IMeetingTaskService _MeetingTaskService;
        private IHubContext<MeetingHub> _hub;

        public MeetingTaskController(IMeetingTaskService MeetingTaskService, IHubContext<MeetingHub> hubContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _MeetingTaskService = MeetingTaskService;
            _hub = hubContext;
        }

        /// <summary>
        /// Gets a Task by Id
        /// </summary>
        /// <param name="ID">Meeting Task ID</param>
        [HttpGet()]
        public Result Get(Guid ID)
        {
            return _MeetingTaskService.Get(ID);
        }

        [HttpPost()]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            return _MeetingTaskService.GetAll(dataSource);
        }
        /// <summary>
        /// Creates a meeting task
        /// </summary>
        /// <param name="MeetingTask">Meeting Task Object</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///  {
        ///  "name": "Task 101",
        ///  "meetingId": "bcac6e40-35b6-4e75-9ef1-628225314a5a",
        ///  "assigneeId": "e004b21b-d340-45de-a3e1-3a2cf248cee8",
        ///  "status": 0,
        ///  "dueDate": "2019-07-25T12:49:00.822Z"
        ///}
        /// </remarks>
        [HttpPost]
        public Result Create([FromBody]MeetingTaskDto MeetingTask)
        {
            Result result = _MeetingTaskService.Create(MeetingTask);
            _hub.Clients.All.SendAsync("addTask", result);
            return result;

        }

        /// <summary>
        /// Updates a meeting task
        /// </summary>
        /// <param name="MeetingTask">Meeting Task Object</param>
        /// <remarks>
        /// Sample request:
        ///     
        ///     {  "Id":
        ///        "Name":
        ///        "Status":
        ///        "MeetingId": 
        ///        "PresenterId": 
        ///        "AssigneeId":
        ///        "DueDate":
        ///     }
        /// </remarks>
        [HttpPut]
        public Result Update(MeetingTaskDto MeetingTask)
        {
            return _MeetingTaskService.Update(MeetingTask);
        }


        [HttpPost]
        public Result CloseTask(Guid id)
        {
            Result result = _MeetingTaskService.CloseTask(id);
            _hub.Clients.All.SendAsync("closeTask", result);
            return result;
        }

        /// <summary>
        /// Deletes a meeting task
        /// </summary>
        /// <param name="id">Meeting Task id</param>
        [HttpDelete]
        public Result DeleteMeetingTask(Guid id)
        {
            return _MeetingTaskService.Delete(id);
        }

    }
}