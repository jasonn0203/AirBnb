using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            // Sắp xếp danh sách phòng theo thứ tự ngẫu nhiên mỗi khi reload
            Random random = new Random();
            phongList = phongList.OrderBy(x => random.Next()).ToList();

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






        //-------------------------------------------
        //Đăng ký

        [HttpGet]
        public ActionResult SignUp()
        {
            //Check khách có đăng nhập chưa , nếu có chuyển về trang chủ
            if (Session["KhachThue"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(KhachThue cust)
        {
            if (ModelState.IsValid)
            {
                //TH tên trống
                if (string.IsNullOrEmpty(cust.Ten))
                    ModelState.AddModelError(string.Empty, "Name cannot be blank!");

                //TH địa chỉ trống
                if (string.IsNullOrEmpty(cust.DiaChi))
                    ModelState.AddModelError(string.Empty, "Address cannot be blank!");



                //TH Email 
                if (string.IsNullOrEmpty(cust.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email cannot be blank!");
                }
                else if (!IsValidEmail(cust.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email is not in a valid format!");

                }
                //TH Mật khẩu
                if (string.IsNullOrEmpty(cust.MatKhau))
                {
                    ModelState.AddModelError(string.Empty, "Password cannot be blank!");
                }
                else if (cust.MatKhau.Length > 8)
                {
                    ModelState.AddModelError(string.Empty, "Password must not longer than 8 characters!");
                }

                //TH SĐT
                if (string.IsNullOrEmpty(cust.SDT))
                    ModelState.AddModelError(string.Empty, "Phone number cannot be blank!");
                else if (cust.SDT.Length > 11)
                {
                    ModelState.AddModelError(string.Empty, "Phone number must not longer than 11 numbers!");

                }

            }

            TryValidateModel(cust);



            //KT trường hợp trùng email đky tồn tại trong db
            var checkCust = db.KhachThues.FirstOrDefault(h => h.Email.Equals(cust.Email));

            //TH có trùng
            if (checkCust != null)
            {
                ModelState.AddModelError(string.Empty, "Please use another email, this email is taken!");
            }

            if (ModelState.IsValid)
            {
                db.KhachThues.Add(cust);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    // In ra thông tin chi tiết về lỗi
                    foreach (var errorMessage in errorMessages)
                    {
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }

                    return View();
                }
            }
            else
            {
                return View();
            }

            return RedirectToAction("SignIn");
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

        //Đăng nhập
        [HttpGet]
        public ActionResult SignIn()
        {
            //Check khách có đăng nhập chưa , nếu có chuyển về trang chủ
            if (Session["KhachThue"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(KhachThue cust)
        {
            if (string.IsNullOrEmpty(cust.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is required!");

            }

            if (string.IsNullOrEmpty(cust.MatKhau))
            {
                ModelState.AddModelError(string.Empty, "Password is required!");

            }

            var custCheck = db.KhachThues.FirstOrDefault(c => c.Email == cust.Email && c.MatKhau == cust.MatKhau);
            if (custCheck != null)
            {
                Session["KhachThue"] = custCheck;
                Session.Timeout = 60;
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password!");
                return View();
            }
        }


        //-------------------------------------------


        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session["KhachThue"] = null;

            return RedirectToAction("Index", "Home");
        }
        //-------------------------------------------

        public ActionResult GetCategories()
        {
            var categoriesList = db.DanhMucPhongs.ToList();
            return PartialView(categoriesList);
        }

        //-------------------------------------------
        //SEARCH bài viết theo tên

        [HttpPost]
        public ActionResult Search(string searchString)
        {
            // Lấy danh sách bài viết từ cơ sở dữ liệu dựa trên từ khóa tìm kiếm
            List<Phong> phongList = db.Phongs.Where(p => p.TieuDe.Contains(searchString) || p.DiaChi.Contains(searchString)).ToList();

            if (phongList.Count == 0)
            {
                return RedirectToAction("NotFound");
            }
            return View("Index", phongList);
        }

        public ActionResult PriceSlider()
        {
            // Lấy giá trị giá thấp nhất và giá cao nhất từ database 
            decimal minPrice = db.Phongs.Min(p => p.Gia1Ngay);
            decimal maxPrice = db.Phongs.Max(p => p.Gia1Ngay);

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            // Trả số lượng phòng
            int numberOfRooms = getNumberOfRoomsByPriceFilter(maxPrice);
            ViewBag.NumberOfRooms = numberOfRooms;

            return PartialView();
        }

        //Lọc giá 
        [HttpPost]
        public ActionResult FilterByPrice(decimal maxPrice)
        {
            // Lấy danh sách phòng theo dk lọc
            List<Phong> filteredRooms = getRoomByPriceFilter(maxPrice);

            // Update số lượng phòng
            int numberOfRooms = filteredRooms.Count();
            ViewBag.NumberOfRooms = numberOfRooms;

            // Return view
            return View("Index", filteredRooms);
        }

        private List<Phong> getRoomByPriceFilter(decimal maxPrice)
        {
            return db.Phongs.Where(p => p.Gia1Ngay <= maxPrice).ToList();
        }

        private int getNumberOfRoomsByPriceFilter(decimal maxPrice)
        {
            // Lấy danh sách phòng theo dk lọc
            List<Phong> filteredRooms = getRoomByPriceFilter(maxPrice);

            // trả về số lượng phòng
            return filteredRooms.Count();
        }

    }
}