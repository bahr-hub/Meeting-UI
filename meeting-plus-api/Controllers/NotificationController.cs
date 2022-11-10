using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetingPlus.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Model.DTO;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiController]
    public class NotificationController : BaseController
    {
        private IHubContext<NotificationHub> _hub;
        private AppSettings _appSettings;
        private INotificationService _NotificationService;

        public NotificationController(IOptions<AppSettings> AppSettings,
            IHubContext<NotificationHub> hub,
            INotificationService NotificationService,
            IUserService UserService,
            IAbilityService AbilityService,
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment)
            : base(UserService, AbilityService, httpContextAccessor, hostingEnvironment)
        {
            _appSettings = AppSettings.Value;
            _NotificationService = NotificationService;
            this._hub = hub;
        }

        [HttpGet]
        public Result Get(Guid ID)
        {
            return _NotificationService.Get(ID);
        }

        [HttpPost]
        public Result GetAll([FromBody]DataSource dataSource)
        {
            //_hub.Clients.All.SendAsync("broadcastnotification", _NotificationService.GetAll(dataSource));

            return _NotificationService.GetAll(dataSource);
        }

        [HttpPost]
        public Result Create()
        {
            return _NotificationService.Create(new NotificationDto() { Id = Guid.NewGuid(), Message = "Test", FkUserId = CurrentUser.Id, DateTime = DateTime.UtcNow });
        }

        [HttpPost]
        public Result Update(NotificationDto notification)
        {
            return _NotificationService.Update(notification);
        }


        [HttpPost]
        public Result SendEmail()
        {
            var template = _appSettings.EmailTemplates.FirstOrDefault(x => x.Name.Equals("welcome"));
            var Params = new Dictionary<string, string>()
            {
                { "UserName", "Eman Reda" }
            };
            return ((BaseService)_NotificationService).SendEmail(new Mail
            {
                From = new UserDto
                {
                    Email = _appSettings.EmailSettings.Email,
                    Name = _appSettings.EmailSettings.Email
                },
                To = new List<UserDto>()
                {
                    new UserDto()
                    {
                        Email = "",
                        Name = ""
                    }
                },
                Subject = "New Meeting has been created.",
                //Text = "New Meeting has been created.",
                Html = ((BaseService)_NotificationService).PopulateBody(string.Concat(((BaseService)_NotificationService)._env.WebRootPath, "/", template.Path), Params),
                Attachments = template.Attachments

            }, template.Path, Params);
        }
    }
}