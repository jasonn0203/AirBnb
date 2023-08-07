using AirBnb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;


namespace AirBnb.Controllers
{
    public class RoomController : Controller
    {
        AirbnbEntities db = new AirbnbEntities();

        //Hiển thị chi tiết phòng
        public ActionResult Detail(string name)
        {
            var roomDetail = db.Phongs.FirstOrDefault(room => room.TieuDe == name);

            List<DanhGia> commentList = new List<DanhGia>();
            if (roomDetail != null)
            {
                commentList = db.DanhGias.Where(dg => dg.MaPhong == roomDetail.MaPhong).ToList();
            }


            ViewBag.CommentList = commentList;




            return View(roomDetail);
        }





        //POST
        [HttpPost]
        public ActionResult AddComment(string comment, int MaPhong, int MaKH, string TieuDe)
        {

            if (Session["KhachThue"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            // Kiểm tra comment không rỗng
            if (string.IsNullOrWhiteSpace(comment))
            {
                // Xử lý lỗi nếu cần thiết
                return RedirectToAction("Detail", new { name = TieuDe });
            }

            // Tạo đối tượng mới để lưu bình luận vào database
            DanhGia newComment = new DanhGia
            {
                NoiDung = comment,
                MaPhong = MaPhong,
                MaKH = MaKH,
                NgayBL = DateTime.Now,
            };

            // Lưu đối tượng mới vào database.
            using (var db = new AirbnbEntities())
            {
                db.DanhGias.Add(newComment);
                db.SaveChanges();
            }

            // Quay trở lại trang chi tiết phòng sau khi thêm bình luận
            return RedirectToAction("Detail", new { name = TieuDe });
        }





    }
}