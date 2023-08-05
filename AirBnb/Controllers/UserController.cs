using AirBnb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

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


        public ActionResult PersonalInfo(string name)
        {
            //Ktra đã đnhap chưa
            if (Session["KhachThue"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            var userInfo = db.KhachThues.FirstOrDefault(c => c.Ten == name);
            return View(userInfo);
        }

        public ActionResult SavePersonalInfo()
        {
            var name = ((KhachThue)Session["KhachThue"]).Ten;
            var userInfo = db.KhachThues.FirstOrDefault(c => c.Ten == name);
            if (userInfo == null)
            {
                return View("NotFound", "Home");
            }

            // Get the new values of the name and phone fields from the user.
            var newName = Request.Form["name"];
            var phone = Request.Form["phone"];
            var soTk = Request.Form["card-number"];
            var ngayHH = Request.Form["expiry-date"];
            var cvv = Request.Form["cvv-number"];

            // Convert ngayHH to DateTime? and cvv to short?
            DateTime? ngayHHDateTime = null;
            short? cvvShort = null;

            if (!string.IsNullOrEmpty(ngayHH))
            {
                ngayHHDateTime = DateTime.Parse(ngayHH);
            }

            if (!string.IsNullOrEmpty(cvv))
            {
                cvvShort = short.Parse(cvv);
            }


            // Update the database.
            userInfo.Ten = newName;
            userInfo.SDT = phone;
            userInfo.SoTK = soTk;
            userInfo.NgayHH = ngayHHDateTime;
            userInfo.CVV = cvvShort;
            db.Entry(userInfo).State = EntityState.Modified;
            db.SaveChanges();

            ((KhachThue)Session["KhachThue"]).Ten = newName;
            // Redirect the user back to the PersonalInfo page with the updated name parameter.
            return RedirectToAction("PersonalInfo", "User", new { name = newName });
        }



        //--------------Danh sách yêu thích
        public ActionResult Favorites(string name)
        {
            if (Session["KhachThue"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            // Lấy danh sách các phòng thuộc mã chủ nhà
            List<Phong> favoriteRoomList =
                db.YeuThiches.Where(y => y.KhachThue.Ten == name)
                 .Select(y => y.Phong)
                 .ToList();



            return View(favoriteRoomList);
        }


        [HttpPost]
        public ActionResult AddToFavorites(int MaPhong)
        {
            //Ktra đã đnhap chưa
            if (Session["KhachThue"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            // Kiểm tra xem mã phòng đã tồn tại trong danh sách yêu thích của khách thuê chưa
            if (!IsFavorite(MaPhong))
            {
                // Nếu chưa tồn tại, thêm mã phòng vào bảng YeuThich
                AddFavorite(MaPhong);
            }
            var name = ((KhachThue)Session["KhachThue"]).Ten;


            return RedirectToAction("Favorites", "User", new { name = name });
        }


        private void AddFavorite(int MaPhong)
        {
            // Lấy thông tin khách thuê hiện tại ( Session )
            var maKH = ((KhachThue)Session["KhachThue"]).MaKH;

            // Thực hiện thêm vào bảng YeuThich
            using (var context = new AirbnbEntities())
            {
                var favorite = new YeuThich
                {
                    MaPhong = MaPhong,
                    MaKH = maKH
                };

                context.YeuThiches.Add(favorite);
                context.SaveChanges();
            }
        }

        //Kiểm tra xem phòng đã tồn tại trong ds yêu thích chưa
        private bool IsFavorite(int MaPhong)
        {
            // Lấy thông tin khách thuê hiện tại ( Session )
            var maKH = ((KhachThue)Session["KhachThue"]).MaKH;

            // Kiểm tra xem mã phòng đã tồn tại trong danh sách yêu thích của khách thuê chưa
            using (var context = new AirbnbEntities())
            {
                var currentCustomerId = maKH; // Lấy mã khách thuê hiện tại từ Session

                return context.YeuThiches.Any(y => y.MaPhong == MaPhong && y.MaKH == currentCustomerId);
            }
        }


        //-------------------------
        //Remove DS yêu thích
        public ActionResult RemoveFromFavorites(int MaPhong)
        {
            // Lấy mã khách hàng hiện tại từ session
            int maKH = ((KhachThue)Session["KhachThue"]).MaKH;

            // Kiểm tra xem mã phòng có tồn tại trong danh sách yêu thích của khách hàng hay không
            if (IsFavorite(MaPhong))
            {
                // Nếu tồn tại, thực hiện xóa khỏi bảng YeuThich
                RemoveFavorite(MaPhong, maKH);
            }
            var name = ((KhachThue)Session["KhachThue"]).Ten;


            return RedirectToAction("Favorites", "User", new { name = name });
        }


        private void RemoveFavorite(int MaPhong, int MaKH)
        {
            // Xóa khỏi bảng YeuThich
            using (var context = new AirbnbEntities())
            {
                var favorite = context.YeuThiches.FirstOrDefault(y => y.MaPhong == MaPhong && y.MaKH == MaKH);
                if (favorite != null)
                {
                    context.YeuThiches.Remove(favorite);
                    context.SaveChanges();
                }
            }
        }




        //------------------
        //Đặt phòng
        public ActionResult Reservations(string name)
        {

            // Lấy danh sách các phòng thuộc mã chủ nhà
            List<DonDatPhong> reservationList =
                db.DonDatPhongs
                .Where(d => d.KhachThue.Ten == name)
                 .ToList();


            return View(reservationList);
        }

        [HttpPost]
        public ActionResult AddToReservations(DateTime start, DateTime end, int MaPhong)
        {
            //Ktra đã đnhap chưa
            if (Session["KhachThue"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }

            // Tính số ngày thuê
            int SoNgayThue = (int)(end - start).TotalDays;

            // Lấy thông tin về phòng từ CSDL 
            using (var dbContext = new AirbnbEntities())
            {


                var room = dbContext.Phongs.FirstOrDefault(r => r.MaPhong == MaPhong);

                if (room != null)
                {
                    // Convert Gia1Ngay to decimal to ensure proper calculation
                    decimal pricePerNight = room.Gia1Ngay;

                    // Define service fee and cleaning fee as decimal
                    decimal serviceFee = Convert.ToDecimal(15);
                    decimal cleaningFee = Convert.ToDecimal(5);

                    // Tính tổng chi phí (Gia1Ngay * SoNgayThue) + serviceFee + cleaningFee
                    decimal TongChiPhi = (pricePerNight * SoNgayThue) + serviceFee + cleaningFee;



                    // Lưu thông tin đặt phòng vào CSDL
                    DonDatPhong booking = new DonDatPhong
                    {
                        NgayDat = DateTime.Now,
                        TongChiPhi = TongChiPhi,
                        SoNgayThue = SoNgayThue,
                        MaPhong = MaPhong,
                        MaKH = ((KhachThue)Session["KhachThue"]).MaKH

                    };


                    dbContext.DonDatPhongs.Add(booking);
                    dbContext.SaveChanges();

                    room.TinhTrang = true;
                    dbContext.SaveChanges();

                    //Chuyển hướng trang thanh toán
                    //Lấy id phòng đặt
                    int reservationId = booking.MaDon;

                    Session["ReservationId"] = reservationId; // Lưu vào Session
                    Session["TongChiPhi"] = TongChiPhi; // Lưu vào Session

                    //TempData["ReservationId"] = reservationId;
                    return RedirectToAction("Stay", "Book", new { reservationId = reservationId });
                }
            }

            var name = ((KhachThue)Session["KhachThue"]).Ten;
            // Redirect về trang hiển thị thông tin đặt phòng
            return RedirectToAction("Reservations", "User", new { name = name });
        }








        //Hủy đặt phòng
        [HttpPost]
        public ActionResult CancelReservations(int MaPhong)
        {

            // Get the current logged-in customer's ID
            int MaKH = ((KhachThue)Session["KhachThue"]).MaKH;

            // Call the CancelReservations method to cancel the reservation
            CancelReservationsAction(MaPhong, MaKH);

            //Set lại tình trạng của phòng
            using (var dbContext = new AirbnbEntities())
            {
                var room = dbContext.Phongs.FirstOrDefault(r => r.MaPhong == MaPhong);
                if (room != null)
                {
                    room.TinhTrang = false;
                    dbContext.SaveChanges();
                }
            }



            var name = ((KhachThue)Session["KhachThue"]).Ten;
            // Redirect về trang hiển thị thông tin đặt phòng
            return RedirectToAction("Reservations", "User", new { name = name });
        }

        //Hủy đặt phòng
        private void CancelReservationsAction(int MaPhong, int MaKH)
        {
            // Xóa khỏi bảng Đơn đặt phòng
            using (var context = new AirbnbEntities())
            {
                var reservationCancel = context.DonDatPhongs.FirstOrDefault(y => y.MaPhong == MaPhong && y.MaKH == MaKH);
                if (reservationCancel != null)
                {
                    // Xóa ràng buộc
                    var associatedHoaDon = context.HoaDons.Where(h => h.MaDon == reservationCancel.MaDon).ToList();
                    foreach (var hoaDon in associatedHoaDon)
                    {
                        context.HoaDons.Remove(hoaDon);
                    }




                    context.DonDatPhongs.Remove(reservationCancel);
                    context.SaveChanges();
                }
            }
        }

        //---------
        //Hiển thị Bill
        public ActionResult Bills(string name)
        {

            // Lấy danh sách các phòng thuộc mã chủ nhà
            List<HoaDon> billLists =
                db.HoaDons
                .Where(d => d.KhachThue.Ten == name)
                 .ToList();


            return View(billLists);
        }




    }
}