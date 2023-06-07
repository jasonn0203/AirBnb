using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirBnb.Controllers
{
    public class RoomController : Controller
    {
        // GET: Room
        public ActionResult Detail()
        {
            return View();
        }
    }
}