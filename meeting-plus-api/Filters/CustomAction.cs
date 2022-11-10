using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared;
using static Shared.SystemEnums;

namespace WebApp.Filters
{
    public class CustomActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                
            }
        }
        public override void OnResultExecuted(ResultExecutedContext context)
        {

            base.OnResultExecuted(context);
        }
    }
}