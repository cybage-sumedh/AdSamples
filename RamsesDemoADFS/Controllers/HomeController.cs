using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using RamsesDemoADFS.Common;
using RamsesDemoADFS.Helpers;

namespace RamsesDemoADFS.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //is this a new session
            if (HttpContext.Session != null && HttpContext.Session.IsNewSession)
            {
                const string issuer = "urn:net-desktop";

                var identity = new ClaimsIdentity(
                  new List<Claim>
                  {
                    new Claim(ClaimTypes.Email, User.Identity.GetEmail(), ClaimValueTypes.String, issuer)
                  },
                  "Cookie",
                  ClaimTypes.Email,
                  ClaimTypes.Role
                  );
                var ctx = Request.GetOwinContext();
                ctx.Authentication.SignIn(identity);
            }

            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View("Error");
        }
    }
}