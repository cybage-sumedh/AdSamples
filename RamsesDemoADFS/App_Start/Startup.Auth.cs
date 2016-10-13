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

// The following using statements were added for this sample.
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Claims;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Interop;
using Microsoft.Owin.Security.Notifications;
using RamsesDemoADFS.Helpers;

namespace RamsesDemoADFS
{
    public partial class Startup
    {
        private static string realm = ConfigurationManager.AppSettings["ida:RPIdentifier"];
        private static string metadata = string.Format("https://{0}/federationmetadata/2007-06/federationmetadata.xml", ConfigurationManager.AppSettings["ida:ADFS"]);


        /// <summary>
        /// Configures the authentication.
        /// </summary>
        /// <param name="app">The application.</param>
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            


            // We need to configure data protection to use a specific key directory
            // we can share between applications.
            //
            // We also need a common protector purpose, as different purposes are
            // automatically isolated from one another.
            //
            // Finally we need to wire up a common ticket formatter.

            // Normally you'd just have a hard coded, or configuration based path,
            // but for this demo we're going to share a directory in the solution directory,
            // so we have to do some jiggery-pokery to figure it out.
            //
            // Let's get ugly to get the current runtime directory. OWIN doesn't give us a
            // way to figure this out, so we're resorting to AppDomain.
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string keyRingPath = Path.GetFullPath(Path.Combine(baseDirectory, "..", "keys"));

            // Now we create a data protector, with a fixed purpose and sub-purpose used in key derivation.
            var protectionProvider = DataProtectionProvider.Create(new DirectoryInfo(keyRingPath));
            var dataProtector = protectionProvider.CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                "Cookie",
                "v2");
            // And finally create a new auth ticket formatter using the data protector.
            var ticketFormat = new AspNetTicketDataFormat(new DataProtectorShim(dataProtector));

            // Now configure the cookie options to have the same cookie name, and use
            // the common format.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookie",
                CookieName = ".AspNet.SharedCookie",
                TicketDataFormat = ticketFormat,
            });

            app.UseWsFederationAuthentication(
                new WsFederationAuthenticationOptions
                {
                    Wtrealm = realm,
                    MetadataAddress = metadata,
                    Notifications = new WsFederationAuthenticationNotifications
                    {
                        AuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("Home/Error?message=" + context.Exception.Message);
                            return Task.FromResult(0);
                        },
                        //SecurityTokenValidated = token =>
                        //{
                        //    //SetCookie(token);
                        //    return Task.FromResult(0);
                        //}
                    }
                });
        }

        private void SetCookie(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> token)
        {
            const string issuer = "urn:net-desktop";

            var identity = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimTypes.Email, token.AuthenticationTicket.Identity.GetEmail(),
                        ClaimValueTypes.String, issuer)
                },
                "Cookie",
                ClaimTypes.Email,
                ClaimTypes.Role
                );
            var ctx = token.OwinContext;
            ctx.Authentication.SignIn(identity);
            
        }
    }
}