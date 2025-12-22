using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    // Bảo vệ toàn bộ Controller: Chỉ giảng viên mới được vào
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private void SetupUserContext()
        {
            // 1. Lấy UserId (được ánh xạ từ "userId" trong API sang NameIdentifier ở LoginController)
            ViewBag.CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

            // 2. Lấy Tên hiển thị (Username)
            ViewBag.CurrentUserName = User.Identity?.Name ?? "Giảng viên";

            // 3. Lấy Token JWT (Dùng để gọi các API yêu cầu xác thực Bearer Token)
            ViewBag.AccessToken = User.FindFirst("JwtToken")?.Value ?? "";
        }

        // --- CÁC TRANG CHÍNH ---

        public IActionResult Dashboard()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Message()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Timetable()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Assignment()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Profile()
        {
            SetupUserContext();
            return View();
        }

        // --- CÁC TRANG PHỤ ---

        public IActionResult Announcement()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Notification()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Settings()
        {
            SetupUserContext();
            return View();
        }

        public IActionResult Help()
        {
            SetupUserContext();
            return View();
        }
    }
}