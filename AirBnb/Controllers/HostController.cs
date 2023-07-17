using System;
using System.Collections.Generic;
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
            return View();
        }

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


            //List<DanhMucPhong> danhMucList = db.DanhMucPhongs.ToList();
            //List<KhuyenMai> khuyenMaiList = db.KhuyenMais.ToList();

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc");
            ViewBag.MaKM = new SelectList(db.KhuyenMais, "MaKM", "TenKM");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRoom([Bind(Include = "TieuDe, DiaChi, MoTa, Gia1Ngay, DieuKhoan, SLKhach, NgayBatDau, NgayKetThuc, MaDanhMuc, MaKM, HinhAnh1,HinhAnh2,HinhAnh3,HinhAnh4,HinhAnh5, TinhTrang")] Phong phong)
        {
            if (ModelState.IsValid)
            {
                // Lấy đối tượng ChuNha từ session
                var chuNha = (ChuNha)Session["ChuNha"];

                // Gán MaChuNha từ đối tượng chuNha vào phong
                phong.MaChuNha = chuNha.MaChuNha;




                // Thực hiện lưu thông tin phòng vào cơ sở dữ liệu
                db.Phongs.Add(phong);
                db.SaveChanges();

                return RedirectToAction("RoomManagement");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc", phong.MaDanhMuc);
            ViewBag.MaKM = new SelectList(db.KhuyenMais, "MaKM", "TenKM", phong.MaKM);
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


            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc", room.MaDanhMuc);
            ViewBag.MaKM = new SelectList(db.KhuyenMais, "MaKM", "TenKM", room.MaKM);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRoom([Bind(Include = "MaPhong, TieuDe, DiaChi, MoTa, Gia1Ngay, DieuKhoan, SLKhach, NgayBatDau, NgayKetThuc, MaDanhMuc, MaKM, HinhAnh1, HinhAnh2, HinhAnh3, HinhAnh4, HinhAnh5, TinhTrang")] Phong phong)
        {
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

                db.SaveChanges();
                return RedirectToAction("RoomManagement");
            }

            ViewBag.MaDanhMuc = new SelectList(db.DanhMucPhongs, "MaDanhMuc", "TenDanhMuc", phong.MaDanhMuc);
            ViewBag.MaKM = new SelectList(db.KhuyenMais, "MaKM", "TenKM", phong.MaKM);

            return View(phong);
        }

        //---------------------------------------------------
        public ActionResult DeleteRoom(int? id)
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

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Phong room = db.Phongs.Find(id);
            db.Phongs.Remove(room);
            db.SaveChanges();
            return RedirectToAction("RoomManagement");
        }

    }
}