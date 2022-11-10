using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPlus.API.Filters
{
  
    public class AuthenticationAttribute : ActionFilterAttribute
    {

        //  Your Properties in Action Filter
        public int MenuID { get; set; }
        public int PriviledgeID { get; set; }
        //public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        //{
        //    //var UserID = HttpContext.Current.User.Identity.GetUserId();
        //    //UsersBLL obj = new UsersBLL();
        //    //MenuBLL menubll = new MenuBLL();
        //    //string ErrMsg = "";

        //    //var Result = obj.CheckPermission(MenuID, PriviledgeID, UserID);
        //    //if (Result.Count > 0) return;
        //    //else
        //    //{
        //    //    AspNetMenu Menu = menubll.GetMenuById(MenuID);

        //    //    if (Menu != null) ErrMsg = Menu.menuName;
        //    //    actionContext.Response = new HttpResponseMessage()
        //    //    {

        //    //        Content = new StringContent(ErrMsg),
        //    //        StatusCode = HttpStatusCode.Forbidden
        //    //    };
        //    //}
        //}
    }
}
