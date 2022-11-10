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
using System.Linq;
using static Shared.SystemEnums;

namespace Services
{
    public interface IAbilityService : IBaseService
    {
        bool Can(string Action, string Controller);
    }

    public class AbilityService : BaseService, IAbilityService
    {

        public AbilityService(IOptions<AppSettings> AppSettings, IMapper Mapper, BaseDataBaseContext Context, IHttpContextAccessor contextAccessor, IHostingEnvironment env)
            : base(AppSettings, Mapper, Context, contextAccessor, env) { WhiteListInitializer(); }
        private Dictionary<string, string> WhiteList = new Dictionary<string, string>();
        public bool Can(string Action, string Controller)
        {
            return (InWhiteList(Action, Controller) || HasRole(Action, Controller));
        }
        private bool HasRole(string Action, string Controller)
        {
            bool has = false;

            var UserRole = _Context.UserRole
                .FirstOrDefault(x => x.FkUserId == CurrentUser.Id);

            if (CurrentUser.IsSuperAdmin == true)
            {
                has = true;
            }
            else if (UserRole != null)
            {
                if (Controller == "MeetingTask") Controller = "task";
                var RolePrivileges = _Context.RoleModulePrivilege.Where(x => x.FkRoleId == UserRole.FkRoleId).ToList();
                var ActionPrivilegeMapping = ActionsPrivilegesMapping.FirstOrDefault(x => x.Value.Contains(Action));
                Enum.TryParse(Controller, true, out Modules module);
                var Privilege = (ActionPrivilegeMapping.Key == null) ? null : RolePrivileges.FirstOrDefault(rp => rp.FkPrivilegeId == ActionPrivilegeMapping.Key
                         && rp.FkModuleId == (int)module);
                has = Privilege != null;
            }

            return has;
        }
        private void WhiteListInitializer()
        {
            WhiteList.Add("Authenticate", "account");
            WhiteList.Add("ChangeUserPassword", "user");
            WhiteList.Add("Get", "SystemConfiguration");
            WhiteList.Add("Notification", "Notification");
            WhiteList.Add("GetAll", "MeetingTask");
            WhiteList.Add("GetAllMeetings", "Meeting");
            WhiteList.Add("GetFiltredMeeting", "Meeting");
            WhiteList.Add("GetAllLite", "role");
            WhiteList.Add("GetModules", "role");
            WhiteList.Add("GetLocations", "Meeting");
            WhiteList.Add("Start", "Meeting");
            WhiteList.Add("accept", "Meeting");
            WhiteList.Add("GetAllMeetingLite", "Meeting");
            WhiteList.Add("end", "Meeting");
            WhiteList.Add("join", "Meeting");
            WhiteList.Add("decline", "Meeting");
            WhiteList.Add("ProposeNewTime", "Meeting");
            WhiteList.Add("AddNote", "Meeting");
            WhiteList.Add("integrate", "User");
            WhiteList.Add("Create", "MeetingTopic");
            WhiteList.Add("AddParticipants", "meeting");
            WhiteList.Add("CloseTask", "MeetingTask");
            WhiteList.Add("CloseTopic", "MeetingTopic");
            WhiteList.Add("DeleteMeetingTopic", "MeetingTopic");
            WhiteList.Add("Encrypt", "Liecense");
            WhiteList.Add("IsValidLiecense", "Liecense");
            WhiteList.Add("DeclineProposal", "Meeting");
            WhiteList.Add("AcceptProposal", "Meeting");

        }
        private bool InWhiteList(string Action, string Controller)
        {
            var IsExist = WhiteList.FirstOrDefault(x => (x.Key.ToLower().Equals(Action.ToLower()) &&
                x.Value.ToLower().Equals(Controller.ToLower())) || (x.Key.ToLower() == x.Value.ToLower() && x.Value.ToLower().Equals(Controller.ToLower())));


            return !string.IsNullOrEmpty(IsExist.Key);
        }
    }
}

