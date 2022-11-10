using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Services;
using Shared;

namespace WebApp.Controllers
{
    [ApiController]
    public class SystemConfigurationController : BaseController
    {
        ISystemConfigurationService _SystemConfigurationService;
        public SystemConfigurationController(ISystemConfigurationService SystemConfigurationService, IHttpContextAccessor httpContextAccessor)
        {
            _SystemConfigurationService = SystemConfigurationService;
        }

        [HttpGet()]
        public Result Get()
        {
            return _SystemConfigurationService.Get();
        }

        [AllowAnonymous]
        [HttpGet()]
        public Result GetAuthenticationMode()
        {
            return _SystemConfigurationService.GetAuthenticationMode();
        }
        [HttpPut]
        public Result Update(SystemConfiguration SystemConfiguration)
        {
            return _SystemConfigurationService.Update(SystemConfiguration);
        }

    }
}