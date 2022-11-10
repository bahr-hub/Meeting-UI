using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Shared;
using WebApp.Controllers;

namespace MeetingPlus.API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class LiecenseController : BaseController
    {
        private ILiecenseService _LiecenseService;
        public LiecenseController(ILiecenseService LiecenseService, IUserService UserService, IAbilityService AbilityService,
            IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
            : base(UserService, AbilityService, httpContextAccessor, hostingEnvironment)
        {
            _LiecenseService = LiecenseService;

        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [AllowAnonymous]
        [HttpPost]
        public Result Encrypt(string value)
        {
            return new Success(_LiecenseService.Encrypt(value));
        }
        [ApiExplorerSettings(IgnoreApi = false)]
        [AllowAnonymous]
        [HttpGet()]
        public Result IsValidLiecense()
        {
            return new Success(_LiecenseService.IsValidLiecense());
        }
        

    }
}