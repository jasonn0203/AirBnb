﻿using System;
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


        public ActionResult PersonalInfo(string name)
        {
            var userInfo = db.KhachThues.FirstOrDefault(c => c.Ten == name);
            return View(userInfo);
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

            ViewBag.SoNgayThue = ViewData["SoNgayThue"] as int?;
            ViewBag.TongChiPhi = ViewData["TongChiPhi"] as decimal?;
            ViewBag.NgayDat = ViewData["NgayDat"] as DateTime?;

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
                    // Tính tổng chi phí ( 15 : service fee, 5 : Cleaning fee
                    decimal TongChiPhi = room.Gia1Ngay * SoNgayThue + 15 + 5;

                    // Lưu thông tin đặt phòng vào CSDL
                    DonDatPhong booking = new DonDatPhong
                    {
                        NgayDat = DateTime.Now,
                        TongChiPhi = TongChiPhi,
                        SoNgayThue = SoNgayThue,
                        MaPhong = MaPhong,
                        MaKH = ((KhachThue)Session["KhachThue"]).MaKH

                    };


                    ViewData["TongChiPhi"] = ViewBag.TongChiPhi;
                    ViewData["SoNgayThue"] = ViewBag.SoNgayThue;
                    ViewData["NgayDat"] = ViewBag.NgayDat;


                    dbContext.DonDatPhongs.Add(booking);
                    dbContext.SaveChanges();
                }
            }

            var name = ((KhachThue)Session["KhachThue"]).Ten;
            // Redirect về trang hiển thị thông tin đặt phòng
            return RedirectToAction("Reservations", "User", new { name = name });
        }


    }
}