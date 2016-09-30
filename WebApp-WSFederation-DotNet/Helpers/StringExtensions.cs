using Microsoft.IdentityModel.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace WebApp_WSFederation_DotNet.Helpers
{
    public static class StringExtensions
    {
        public static string GetShortNameFromEmail(this IIdentity identity)
        {

            if (identity == null)
            {
                return string.Empty;
            }
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)identity;
            var emailClaim = claimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            if (string.IsNullOrEmpty(emailClaim?.Value))
            {
                return string.Empty;
            }
            var atIndex = emailClaim.Value.IndexOf('@');
            return atIndex >= 0 ? emailClaim.Value.Substring(0, atIndex) : emailClaim.Value;
        }
    }
}