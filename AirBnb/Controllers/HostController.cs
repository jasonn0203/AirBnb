using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirBnb.Controllers
{
    public class HostController : Controller
    {
        // GET: Host
        public ActionResult HostSignUp()
        {
            return View();
        }

        public ActionResult HostSignIn()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}