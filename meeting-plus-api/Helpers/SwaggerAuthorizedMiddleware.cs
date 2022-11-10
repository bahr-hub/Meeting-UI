using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp
{
    public class SwaggerAuthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAuthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                if (!context.User.Identity.IsAuthenticated
                    || (context.User.Identity.IsAuthenticated && context.User.Claims.FirstOrDefault(x => x.Type == "IsSuperAdmin").Value != "True"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
