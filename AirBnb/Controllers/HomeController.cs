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
            // Lấy tất cả danh sách phòng
            List<Phong> phongList = db.Phongs.ToList();
            return View(phongList);
        }



        public ActionResult RoomByCategory(string categoryName)
        {
            var danhMucPhong = db.DanhMucPhongs.FirstOrDefault(d => d.TenDanhMuc == categoryName);
            if (danhMucPhong == null)
            {
                return View("Index");
            }

            var roomItemByCategoryList = db.Phongs.Where(product => product.MaDanhMuc == danhMucPhong.MaDanhMuc).ToList();
            if (roomItemByCategoryList.Count == 0)
            {
                return RedirectToAction("NotFound");
            }

            return View("Index", roomItemByCategoryList);
        }



        public ActionResult AirCover()
        {
            return View();
        }

        public ActionResult NotFound()
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