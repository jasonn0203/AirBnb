using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirBnb.Models;

namespace AirBnb.Controllers
{
    public class UserController : Controller
    {

        AirbnbEntities db = new AirbnbEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(string hostName)
        {
            var hostInfo = db.ChuNhas.FirstOrDefault(host => host.Ten == hostName);

            if (hostInfo == null)
            {
                // Xử lý khi không tìm thấy thông tin chủ nhà
                return RedirectToAction("NotFound", "Home");
            }

            return View(hostInfo);
        }


        public ActionResult PersonalInfo()
        {
            return View();
        }
    }
}