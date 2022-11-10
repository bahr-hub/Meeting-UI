using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class AccountController : BaseController
    {
        private IAccountService _AccountService;
        public AccountController(IAccountService AccountService, IUserService UserService, IAbilityService AbilityService,
            IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
            : base(UserService, AbilityService, httpContextAccessor, hostingEnvironment)
        {
            _AccountService = AccountService;
            _UserService = UserService;
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [AllowAnonymous]
        [HttpPost]
        public Result Authenticate([FromBody] UserLogin User)
        {
            return _AccountService.Authenticate(User.Email, User.Password, User.UserDate);
        }

    }
}