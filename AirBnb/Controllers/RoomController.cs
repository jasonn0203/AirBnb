using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirBnb.Models;

namespace AirBnb.Controllers
{
    public class RoomController : Controller
    {
        AirbnbEntities db = new AirbnbEntities();

        // Hiển thị chi tiết phòng
        public ActionResult Detail(int id)
        {
            var roomDetail = db.Phongs.FirstOrDefault(room => room.MaPhong == id);
            

            return View(roomDetail);
        }
    }
}