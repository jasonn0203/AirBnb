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
        public ActionResult Detail(string name)
        {
            var roomDetail = db.Phongs.FirstOrDefault(room => room.TieuDe == name);
            

            return View(roomDetail);
        }




        //Comment
        [HttpPost]
        public ActionResult AddComment(int MaPhong, int MaKH, string NoiDung)
        {
            if (ModelState.IsValid)
            {
                var danhGia = new DanhGia
                {
                    NoiDung = NoiDung,
                    MaPhong = MaPhong,
                    MaKH = MaKH
                };

                db.DanhGias.Add(danhGia);
                db.SaveChanges();

                // Truy vấn danh sách bình luận từ cơ sở dữ liệu
                var comments = db.DanhGias.ToList();

                // Truyền danh sách bình luận vào view để hiển thị
                return View("Detail", comments);
            }

            // Trường hợp ModelState không hợp lệ, xử lý lỗi hoặc thông báo cho người dùng
            return View();
        }



    }
}