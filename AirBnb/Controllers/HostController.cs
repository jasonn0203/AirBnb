using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AirBnb.Models;

namespace AirBnb.Controllers
{
    public class HostController : Controller
    {


        AirbnbEntities db = new AirbnbEntities();

        //-----------------------------------------------------------------

        [HttpGet]
        public ActionResult HostSignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HostSignUp(ChuNha host)
        {
            if (ModelState.IsValid)
            {
                //TH tên trống
                if (string.IsNullOrEmpty(host.Ten))
                    ModelState.AddModelError(string.Empty, "Name cannot be blank!");

                //TH địa chỉ trống
                if (string.IsNullOrEmpty(host.DiaChi))
                    ModelState.AddModelError(string.Empty, "Address cannot be blank!");

                //TH năm KN
                if (!int.TryParse(host.NamKN.ToString(), out int yearsOfExperience))
                {
                    ModelState.AddModelError(string.Empty, "Years of Experience must be a valid integer!");
                }
                else if (string.IsNullOrEmpty(host.NamKN.ToString()))
                {
                    ModelState.AddModelError(string.Empty, "Years of Experience cannot be blank!");
                }

                //TH công việc
                if (string.IsNullOrEmpty(host.CongViec))
                    ModelState.AddModelError(string.Empty, "Job cannot be blank!");

                //TH Email 
                if (string.IsNullOrEmpty(host.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email cannot be blank!");
                }
                else if (!IsValidEmail(host.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email is not in a valid format!");

                }
                //TH Mật khẩu
                if (string.IsNullOrEmpty(host.MatKhau))
                {
                    ModelState.AddModelError(string.Empty, "Password cannot be blank!");
                }
                else if (host.MatKhau.Length > 8)
                {
                    ModelState.AddModelError(string.Empty, "Password must not longer than 8 characters!");
                }

                //TH SĐT
                if (string.IsNullOrEmpty(host.SDT))
                    ModelState.AddModelError(string.Empty, "Phone number cannot be blank!");
                else if (host.SDT.Length > 11)
                {
                    ModelState.AddModelError(string.Empty, "Phone number must not longer than 11 numbers!");

                }

            }

            TryValidateModel(host);



            //KT trường hợp trùng email đky tồn tại trong db
            var checkHost = db.ChuNhas.FirstOrDefault(h => h.Email.Equals(host.Email));

            //TH có trùng
            if (checkHost != null)
            {
                ModelState.AddModelError(string.Empty, "Please use another email, this email is taken!");
            }

            if (ModelState.IsValid)
            {
                db.ChuNhas.Add(host);
                db.SaveChanges();
            }
            else
            {
                return View();
            }

            return RedirectToAction("HostSignIn");
        }

        //ĐK check email đúng định dạng
        private bool IsValidEmail(string email)
        {
            // Pattern Regex của mail
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Check xem định dạng mail nhập vào có trùng với pattern hay ko
            Match match = Regex.Match(email, pattern);

            // Trả về kết quả
            return match.Success;
        }
        //-----------------------------------------------------------------

        [HttpGet]
        public ActionResult HostSignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HostSignIn(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required!");
                return View();
            }

            var host = db.ChuNhas.FirstOrDefault(h => h.Email.Equals(email) && h.MatKhau.Equals(password));
            if (host != null)
            {
                Session["ChuNha"] = host;
                Session.Timeout = 60;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password!");
                return View();
            }
        }

        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session["ChuNha"] = null;



            return RedirectToAction("HostSignIn", "Host");
        }



        //------------------------------------------------------------------

        public ActionResult Dashboard()
        {
            //Check chủ nhà có đăng nhập chưa , nếu chưa chuyển về trang đăng nhập
            if (Session["ChuNha"] == null)
            {
                return RedirectToAction("HostSignIn");
            }

            //Lấy số lượng phòng đã đăng
            var maChuNha = ((ChuNha)Session["ChuNha"]).MaChuNha;
            int roomQuantity = 0;

            roomQuantity = db.Phongs.Count(r => r.MaChuNha == maChuNha);
            ViewBag.SoLuongPhong = roomQuantity;

            //truyền tổng doanh thu
            decimal? totalRevenue = CalculateTotalRevenue(maChuNha);
            ViewBag.TongDoanhThu = totalRevenue;


            //Tổng KH đặt phòng
            int numberOfBookings = GetNumberOfBookings(maChuNha);
            ViewBag.SLDatPhong = numberOfBookings;


            List<Phong> rooms = GetRoomList(maChuNha);

            //Lấy comments
            var comments = GetCommentsByRoomForHost(maChuNha);
            int totalReviews = GetTotalReviews(rooms);

            ViewBag.Comments = comments;
            ViewBag.TotalReviews = totalReviews;



            return View();
        }


        private decimal? CalculateTotalRevenue(int hostId)
        {
            // Tính tổng doanh thu
            decimal? totalRevenue = db.DonDatPhongs
                .Where(d => d.Phong.MaChuNha == hostId)
                .Sum(d => (decimal?)d.TongChiPhi); // Use (decimal?) to convert to nullable decimal

            return totalRevenue;
        }






        private int GetNumberOfBookings(int hostId)
        {
            // Tính SL người đã đặt phòng

            int numberOfBookings = db.DonDatPhongs
                .Count(d => d.Phong.MaChuNha == hostId);

            return numberOfBookings;
        }


        private List<Phong> GetCommentsByRoomForHost(int hostId)
        {
            // Lấy các đánh giá của các căn phòng thuộc chủ nhà
            List<Phong> rooms = GetRoomList(hostId);

            return rooms;
        }

        private int GetTotalReviews(List<Phong> rooms)
        {
            // Tính tổng số lượng đánh giá của tất cả các phòng
            int totalReviews = rooms.Sum(room => room.DanhGias.Count);
            return totalReviews;
        }

        private List<Phong> GetRoomList(int hostId)
        {
            return db.Phongs
                .Where(p => p.MaChuNha == hostId)
                .Include(p => p.DanhGias.Select(d => d.KhachThue))
                .ToList();
        }


        //===============================
        //Chart
        public ActionResult RoomChart(int maChuNha)
        {
            List<Phong> rooms = new List<Phong>();
            using (var dbContext = new AirbnbEntities())
            {
                rooms = dbContext.Phongs.Where(r => r.MaChuNha == maChuNha).ToList();
            }

            // Xử lý dữ liệu để lấy số lượng thuê phòng theo ngày đặt
            var rentCountByDate = rooms.Join(db.DonDatPhongs,
                                  r => r.MaPhong,
                                  d => d.MaPhong,
                                  (r, d) => new { Date = d.NgayDat, Count = 1 })
                            .GroupBy(rd => rd.Date)
                            .Select(g => new { Date = g.Key, Count = g.Sum(rd => rd.Count) })
                            .Distinct()
                            .OrderBy(rd => rd.Date)
                            .ToList();


            // Chuyển dữ liệu thành các mảng để sử dụng trong biểu đồ
            var dates = rentCountByDate.Select(rd => rd.Date.ToString("dd-MM-yyyy")).ToArray();
            var counts = rentCountByDate.Select(rd => rd.Count).ToArray();

            // Truyền dữ liệu vào ViewBag để sử dụng trong View
            ViewBag.Dates = dates;
            ViewBag.Counts = counts;

            //Doanh thu chart

            var revenueByDate = db.DonDatPhongs
                           .Where(d => d.Phong.MaChuNha == maChuNha) // Filter dua theo machunha
                           .GroupBy(d => DbFunctions.TruncateTime(d.NgayDat))
                           .Select(g => new { Date = g.Key, Revenue = g.Sum(d => d.TongChiPhi) })
                           .AsEnumerable()
                           .Select(rd => new { Date = rd.Date, rd.Revenue })
                           .ToList();



            var revenueDate = revenueByDate.Select(rd => rd.Date).ToArray();
            var revenues = revenueByDate.Select(rd => rd.Revenue).ToArray();



            ViewBag.RevenueDates = revenueDate;
            ViewBag.Revenue = revenues;

            return View();
        }
















        //===============================


        public ActionResult RoomManagement()
        {
            //Check chủ nhà có đăng nhập chưa , nếu chưa chuyển về trang đăng nhập
            if (Session["ChuNha"] == null)
            {
                return RedirectToAction("HostSignIn");
            }


            // Lấy mã chủ nhà từ session
            var maChuNha = ((ChuNha)Session["ChuNha"]).MaChuNha;

            // Lấy danh sách các phòng thuộc mã chủ nhà
            List<Phong> phongList = db.Phongs.Where(p => p.MaChuNha == maChuNha).ToList();

            if (phongList == null || phongList.Count == 0)
            {
                ViewBag.Message = "No Room was found! Please add a new one.";
                return View();
            }
            return View(phongList);
        }

        //------------------------------------------------------------------
        //Thêm xóa sửa phòng
        public ActionResult CreateRoom()
        {
            // Logic để lấy danh sách danh mục phòng, khuyến mãi (để hiển thị cho form thêm phòng)


            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc");
            // Lấy đối tượng ChuNha từ session
            var chuNha = (ChuNha)Session["ChuNha"];

            // Lấy danh sách khuyến mãi của tài khoản chủ nhà đang đăng nhập
            var kmList = db.ChuNhas
                           .Where(c => c.MaChuNha == chuNha.MaChuNha)
                           .SelectMany(c => c.KhuyenMais)
                           .ToList();

            ViewBag.MaKM = new SelectList(kmList, "MaKM", "TenKM");




            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRoom([Bind(Include = "TieuDe, DiaChi, MoTa, Gia1Ngay, DieuKhoan, SLKhach, NgayBatDau, NgayKetThuc, MaDanhMuc, MaKM, HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4,HinhAnh5, TinhTrang")] Phong phong)
        {
            // Lấy đối tượng ChuNha từ session
            var chuNha = (ChuNha)Session["ChuNha"];
            if (ModelState.IsValid)
            {


                // Gán MaChuNha từ đối tượng chuNha vào phong
                phong.MaChuNha = chuNha.MaChuNha;


                UpdateRentalDates();

                // Thực hiện lưu thông tin phòng vào cơ sở dữ liệu
                db.Phongs.Add(phong);
                db.SaveChanges();

                return RedirectToAction("RoomManagement");
            }

            // Lấy danh sách khuyến mãi của tài khoản chủ nhà đang đăng nhập
            var kmList = db.ChuNhas
                           .Where(c => c.MaChuNha == chuNha.MaChuNha)
                           .SelectMany(c => c.KhuyenMais)
                           .ToList();

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc", phong.MaDanhMuc);
            ViewBag.MaKM = new SelectList(kmList, "MaKM", "TenKM", phong.MaKM);

            // Nếu dữ liệu không hợp lệ, quay trở lại view create để hiển thị lỗi
            return View(phong);
        }

        //-----------------------------------------------
        //Edit
        public ActionResult EditRoom(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Phong room = db.Phongs.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            // Lấy đối tượng ChuNha từ session
            var chuNha = (ChuNha)Session["ChuNha"];

            // Lấy danh sách khuyến mãi của tài khoản chủ nhà đang đăng nhập
            var kmList = db.ChuNhas
                           .Where(c => c.MaChuNha == chuNha.MaChuNha)
                           .SelectMany(c => c.KhuyenMais)
                           .ToList();


            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc", room.MaDanhMuc);
            ViewBag.MaKM = new SelectList(kmList, "MaKM", "TenKM", room.MaKM);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoom([Bind(Include = "MaPhong, TieuDe, DiaChi, MoTa, Gia1Ngay, DieuKhoan, SLKhach, NgayBatDau, NgayKetThuc, MaDanhMuc, MaKM, HinhAnh1, HinhAnh2, HinhAnh3, HinhAnh4, HinhAnh5, TinhTrang")] Phong phong)
        {
            // Lấy đối tượng ChuNha từ session
            var chuNha = (ChuNha)Session["ChuNha"];
            if (ModelState.IsValid)
            {
                var getRoomInDb = db.Phongs.Find(phong.MaPhong);

                if (getRoomInDb == null)
                {
                    return HttpNotFound(); // Hoặc xử lý lỗi theo cách khác
                }

                // Cập nhật các thuộc tính của getRoomInDb từ phong
                getRoomInDb.TieuDe = phong.TieuDe;
                getRoomInDb.DiaChi = phong.DiaChi;
                getRoomInDb.MoTa = phong.MoTa;
                getRoomInDb.Gia1Ngay = phong.Gia1Ngay;
                getRoomInDb.DieuKhoan = phong.DieuKhoan;
                getRoomInDb.SLKhach = phong.SLKhach;
                getRoomInDb.NgayBatDau = phong.NgayBatDau;
                getRoomInDb.NgayKetThuc = phong.NgayKetThuc;
                getRoomInDb.MaDanhMuc = phong.MaDanhMuc;
                getRoomInDb.MaKM = phong.MaKM;
                getRoomInDb.HinhAnh1 = phong.HinhAnh1;
                getRoomInDb.HinhAnh2 = phong.HinhAnh2;
                getRoomInDb.HinhAnh3 = phong.HinhAnh3;
                getRoomInDb.HinhAnh4 = phong.HinhAnh4;
                getRoomInDb.HinhAnh5 = phong.HinhAnh5;
                getRoomInDb.TinhTrang = phong.TinhTrang;

                UpdateRentalDates();

                db.SaveChanges();
                return RedirectToAction("RoomManagement");
            }


            // Lấy danh sách khuyến mãi của tài khoản chủ nhà đang đăng nhập
            var kmList = db.ChuNhas
                           .Where(c => c.MaChuNha == chuNha.MaChuNha)
                           .SelectMany(c => c.KhuyenMais)
                           .ToList();

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc", phong.MaDanhMuc);
            ViewBag.MaKM = new SelectList(kmList, "MaKM", "TenKM", phong.MaKM);

            return View(phong);
        }


        public void UpdateRentalDates()
        {
            using (AirbnbEntities context = new AirbnbEntities())
            {
                context.Database.ExecuteSqlCommand("UpdateRentalDates");
            }
        }


        //---------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoom(int id)
        {
            Phong room = db.Phongs.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            db.Phongs.Remove(room);
            db.SaveChanges();
            return RedirectToAction("RoomManagement");
        }




        public ActionResult Promotion()
        {
            // Check chủ nhà có đăng nhập chưa, nếu chưa chuyển về trang đăng nhập
            if (Session["ChuNha"] == null)
            {
                return RedirectToAction("HostSignIn");
            }

            // Lấy mã chủ nhà từ session
            var maChuNha = ((ChuNha)Session["ChuNha"]).MaChuNha;

            // Lấy danh sách các phòng thuộc mã chủ nhà
            var kmList = db.KhuyenMais
                 .Where(km => km.MaChuNha == maChuNha)
                 .ToList();

            if (kmList == null || kmList.Count == 0)
            {
                ViewBag.Message = "No Promotion was found! Please add a new one.";
                return View();
            }

            return View(kmList);
        }



        public ActionResult CreatePromotion()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePromotion(KhuyenMai promotion)
        {
            if (ModelState.IsValid)
            {
                // Lấy mã chủ nhà từ session
                var maChuNha = ((ChuNha)Session["ChuNha"]).MaChuNha;

                try
                {
                    promotion.MaChuNha = maChuNha;
                    db.KhuyenMais.Add(promotion);
                    db.SaveChanges();
                    return RedirectToAction("Promotion");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while adding the promotion: " + ex.Message;
                }
            }

            return View(promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePromotion(int id)
        {
            KhuyenMai km = db.KhuyenMais.Find(id);
            if (km == null)
            {
                return HttpNotFound();
            }
            db.KhuyenMais.Remove(km);
            db.SaveChanges();
            return RedirectToAction("Promotion");
        }
    }
}