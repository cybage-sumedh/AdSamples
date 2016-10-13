using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace RamsesDemoADFS.Helpers
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
            var nameClaim = claimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            var givenNameClaim = claimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var roleClaim = claimsIdentity.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            if (string.IsNullOrEmpty(emailClaim?.Value))
            {
                return string.Empty;
            }
            var atIndex = emailClaim.Value.IndexOf('@');
            return (atIndex >= 0 ? emailClaim.Value.Substring(0, atIndex) : emailClaim.Value) ;
        }

        public static string GetName(this IIdentity identity)
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

        public static string GetEmail(this IIdentity identity)
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
            return emailClaim.Value;
        }
    }
}