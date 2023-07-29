using AirBnb.Models;
using System;
using System.Web.Mvc;

namespace AirBnb.Controllers
{
    public class BookController : Controller
    {
        AirbnbEntities db = new AirbnbEntities();
        // GET: Book
        public ActionResult Stay()
        {
            //Ktra đã đnhap chưa
            if (Session["KhachThue"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();

        }


        //---------------------------------------
        //Thanh toán

        [HttpPost]
        public ActionResult Checkout(string SoTK, DateTime? NgayHH, short? CVV)
        {
            int maDon = (int)Session["ReservationId"]; // Lấy từ Session

            //// Kiểm tra xem NgayHH và CVV có giá trị null hay không
            //if (!NgayHH.HasValue || !CVV.HasValue)
            //{

            //    return RedirectToAction("Stay", new { reservationId = maDon });
            //}



            using (var dbContext = new AirbnbEntities())
            {
                // Kiểm tra xem MaDon có tồn tại trong cơ sở dữ liệu hay không
                var donDatPhong = dbContext.DonDatPhongs.Find(maDon);
                if (donDatPhong == null)
                {
                    // Nếu MaDon không tồn tại, bạn có thể chuyển hướng hoặc hiển thị thông báo lỗi
                    return RedirectToAction("Stay", new { reservationId = maDon });
                }

                // Lấy thông tin khách hàng từ Session (hoặc bạn có thể lấy thông tin từ HoaDon nếu đã được lưu ở trước đó)
                KhachThue cust = (KhachThue)Session["KhachThue"] as KhachThue;

                // Thêm hóa đơn mới vào cơ sở dữ liệu
                HoaDon hoaDon = new HoaDon();
                hoaDon.MaKH = cust.MaKH;
                hoaDon.MaDon = maDon; // Gán MaDon cho hóa đơn
                hoaDon.NgayTao = DateTime.Now;
                dbContext.HoaDons.Add(hoaDon);
                dbContext.SaveChanges();



                return RedirectToAction("SuccessfullyPayment");
            }
        }





        public ActionResult SuccessfullyPayment()
        {
            return View();
        }
    }
}