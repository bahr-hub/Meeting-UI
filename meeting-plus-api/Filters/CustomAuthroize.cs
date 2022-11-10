using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using static Shared.SystemEnums;
using Shared;

namespace WebApp.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomAuthorization : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var controllerInfo = filterContext.ActionDescriptor as ControllerActionDescriptor;
            var abilityService = filterContext.HttpContext.RequestServices.GetService<IAbilityService>();
            var liecenseService = filterContext.HttpContext.RequestServices.GetService<ILiecenseService>();

            if (filterContext != null)
            {
                string ControllerName = controllerInfo.ControllerName;
                string ActionName = controllerInfo.ActionName;
                //filterContext.HttpContext.User.Claims;
                //filterContext.HttpContext.User.IsInRole
                // filterContext.HttpContext.User.Identity.IsAuthenticated == true;
                var can = abilityService.Can(ActionName, ControllerName);


                if (false)//worked but redirect from browser directly, cannot from swagger or Angular
                    filterContext.Result = new RedirectResult("http://localhost:6251/");
                if (!can)
                    filterContext.Result = filterContext.Result = new StatusCodeResult(401); //new RedirectResult("https://exceptionnotfound.net/asp-net-core-demystified-action-results/");
                if(!liecenseService.IsValidLiecense())
                    filterContext.Result = new ObjectResult(new { statusCode = 707, message = ErrorMsg.LiecenseExpired.ToDescriptionString(), currentDate = DateTime.Now });

                //filterContext.Result = new ContentResult
                //{
                //    StatusCode = (int)System.Net.HttpStatusCode.Redirect,
                //    Content = "Check this URL"
                //};

                //
                //filterContext.Headers.Location = new Uri("https://xxx.xxx.com");


            }
        }

    }
}
