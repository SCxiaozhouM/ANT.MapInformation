using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;

namespace ANT.MapInformation.WebAPI.App_Start
{
    public class JsonNetActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
           //if (actionContext.ActionDescriptor.ActionName!="Login")
           // {
           //     var name = HttpContext.Current.Request.Cookies["name"];
           //     if (name == null)
           //     {
           //         actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK,new { status="error",code=404});
           //     }
           // }
        }
    }
}