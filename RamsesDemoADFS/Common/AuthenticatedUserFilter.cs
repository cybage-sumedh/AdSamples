using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using RamsesDemoADFS.Helpers;

namespace RamsesDemoADFS.Common
{
    public class AuthenticatedUserFilter : ActionFilterAttribute
    {
        //we are overriding OnActionExecuting method since this one
        //is executed prior the controller action method itself
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //is this a new session
            if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session.IsNewSession)
            {

                const string issuer = "urn:net-desktop";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Thread.CurrentPrincipal.Identity.GetName(), ClaimValueTypes.String, issuer),
                    new Claim(ClaimTypes.Email, Thread.CurrentPrincipal.Identity.GetEmail(), ClaimValueTypes.String, issuer)
                };

                var identity = new ClaimsIdentity(
                   claims,
                    "Cookie",
                    ClaimTypes.Email,
                    ClaimTypes.Role
                    );
                var ctx = filterContext.HttpContext.Request.GetOwinContext();
                ctx.Authentication.SignIn(identity);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}