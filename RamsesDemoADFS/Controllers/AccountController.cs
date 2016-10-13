//----------------------------------------------------------------------------------------------
//    Copyright 2014 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// The following using statements were added for this sample.
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading;
using RamsesDemoADFS.Helpers;

namespace RamsesDemoADFS.Controllers
{
    public class AccountController : Controller
    {

        //[ValidateAntiForgeryToken]
        public void SignIn()
        {
            // Send a WSFederation sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" }, WsFederationAuthenticationDefaults.AuthenticationType);

                ////is this a new session
                //if (HttpContext.Session != null && HttpContext.Session.IsNewSession)
                //{
                //    const string issuer = "urn:net-desktop";

                //    var identity = new ClaimsIdentity(
                //      new List<Claim>
                //      {
                //        new Claim(ClaimTypes.Email, User.Identity.GetEmail(), ClaimValueTypes.String, issuer)
                //      },
                //      "Cookie",
                //      ClaimTypes.Email,
                //      ClaimTypes.Role
                //      );
                //    var ctx = HttpContext.Request.GetOwinContext();
                //    ctx.Authentication.SignIn(identity);
                //}
            }
            else
            {
                
            }
        }
        public void SignOut()
        {
            // Send a WSFederation sign-out request.
            HttpContext.GetOwinContext().Authentication.SignOut(
                WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut("Cookie");
        }
	}
}