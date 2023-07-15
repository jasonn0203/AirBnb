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
    }
}