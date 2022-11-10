using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Model.Models;
using Services;
using Shared;
using WebApp.Filters;

namespace WebApp.Controllers
{
    [CustomAction]
    [CustomAuthorization]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public User CurrentUser;
        public IUserService _UserService;
        public IAbilityService _AbilityService;
        public readonly IHostingEnvironment _hostingEnvironment;
        public BaseController(){  }


        public BaseController(IUserService UserService, IAbilityService AbilityService, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _UserService = UserService;
            _AbilityService = AbilityService;
            var ID = httpContextAccessor.HttpContext.User.Identity?.Name;
            CurrentUser = string.IsNullOrEmpty(ID) ? new User() : _UserService.GetCurrentUser(Guid.Parse(ID));
        }

    }
}