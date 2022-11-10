using Microsoft.AspNetCore.SignalR;
using Model.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPlus.API.Hubs
{
    public class MeetingHub : Hub
    {
        //public async Task BroadcastStartMeetingResult(Result data)
        //{
        //    await Clients.All.SendAsync("startMeeting", data);
        //}

    }
}
