using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirBnb.Models;

namespace AirBnb.Controllers
{
    public class HomeController : Controller

    {

        //Get database
        AirbnbEntities db = new AirbnbEntities();


        // GET: Home
        public ActionResult Index()
        {
            //Hiển thị danh sách các phòng
            List<Phong> phongList = db.Phongs.ToList();
            return View(phongList);
        }

        public ActionResult AirCover()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult GetCategories()
        {
            var categoriesList = db.DanhMucPhongs.ToList();
            return PartialView(categoriesList);
        }


    }
}