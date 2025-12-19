using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Diagnostics;
using WebQuanLiKhoaHocApi.Models;

namespace WebQuanLiKhoaHocApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly UniversityDBEntities1 _db;

        public AccountController(UniversityDBEntities1 db)
        {
            _db = db;
        }

        // --- ĐĂNG NHẬP ---
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var userRole = _db.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                if (userRole != null && userRole.RoleName == role)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserRole", userRole.RoleName);

                    return user.RoleId switch
                    {
                        4 => RedirectToAction("Index", "Admin"),
                        3 => RedirectToAction("Index", "Lecturer"),
                        2 => RedirectToAction("Index", "Student"),
                        _ => View()
                    };
                }
            }
            ViewBag.Error = "Sai tài khoản, mật khẩu hoặc vai trò";
            return View();
        }

        // --- QUÊN MẬT KHẨU: BƯỚC 1 - NHẬP EMAIL ---
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                // Tạo OTP 6 số
                string otp = new Random().Next(100000, 999999).ToString();

                // Lưu vào Session (Thống nhất dùng tên "OTP")
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetInt32("ResetUserId", user.UserId);

                // IN RA CỬA SỔ OUTPUT DEBUG
                Debug.WriteLine("========================================");
                Debug.WriteLine($"MA OTP CUA BAN LA: {otp}");
                Debug.WriteLine("========================================");

                return RedirectToAction("VerifyOTP");
            }
            ViewBag.Error = "Email không tồn tại trong hệ thống!";
            return View();
        }

        // --- QUÊN MẬT KHẨU: BƯỚC 2 - NHẬP OTP ---
        [HttpGet]
        public IActionResult VerifyOTP() => View();

        [HttpPost]
        public IActionResult VerifyOTP(string otpInput)
        {
            string sessionOtp = HttpContext.Session.GetString("OTP");
            if (otpInput == sessionOtp && !string.IsNullOrEmpty(sessionOtp))
            {
                return RedirectToAction("ResetPassword");
            }
            ViewBag.Error = "Mã OTP không chính xác!";
            return View();
        }

        // --- QUÊN MẬT KHẨU: BƯỚC 3 - ĐẶT LẠI MẬT KHẨU ---
        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (HttpContext.Session.GetInt32("ResetUserId") == null) return RedirectToAction("ForgotPassword");
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            int? userId = HttpContext.Session.GetInt32("ResetUserId");
            var user = _db.Users.Find(userId);
            if (user != null)
            {
                user.Password = newPassword;
                _db.SaveChanges();
                HttpContext.Session.Clear(); // Xóa session sau khi đổi xong
                return RedirectToAction("Login");
            }
            return RedirectToAction("ForgotPassword");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}