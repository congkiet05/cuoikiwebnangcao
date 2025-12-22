using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebQuanLiKhoaHoc_MVC.Models.Login; // Đã thêm dòng này để dùng ViewModel
using WebQuanLiKhoaHoc_MVC.Service.Login;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthApiService _authApiService;

        public LoginController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì đá về trang chủ
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Gọi API để lấy Token
            var loginResult = await _authApiService.LoginAsync(username, password);

            // Kiểm tra kết quả trả về từ API
            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
                return View("~/Views/Account/Login.cshtml");
            }

            // 2. Giải mã Token để lấy các thông tin ẩn bên trong (UserId,...)
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(loginResult.Token);

            // 3. Tạo danh sách Claims (Dữ liệu định danh cho MVC)
            // Lưu ý: loginResult.Role lấy từ API trả về (Ví dụ: "Admin", "Student", "Lecturer")
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, loginResult.Role), // Quan trọng: MVC dùng cái này để check [Authorize(Roles="...")]
                new Claim("JwtToken", loginResult.Token)      // Lưu token để dành gọi API khác
            };

            // Lấy thêm UserId từ trong token (nếu có) để lưu vào Cookie
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == "sub");
            if (userIdClaim != null)
            {
                claims.Add(new Claim("UserId", userIdClaim.Value));
            }

            // 4. Thiết lập Cookie Authentication
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,            // Giữ đăng nhập khi đóng trình duyệt
                ExpiresUtc = jwtToken.ValidTo   // Cookie hết hạn cùng lúc với Token
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // ============================================================
            // 5. PHÂN LOẠI VÀ CHUYỂN HƯỚNG 
            // ============================================================

            // Kiểm tra Role trả về (Lưu ý: Phải khớp chính xác từng chữ cái với Database)tại
            string userRole = loginResult.Role;

            switch (userRole)
            {
                case "Admin":
                    // Chuyển đến: Areas/Admin/Controllers/HomeController.cs -> Index
                    return RedirectToAction("Dashboard", "Admin");

                case "Student":
                    // Chuyển đến: Areas/Student/Controllers/HomeController.cs -> Index
                    // (Nếu API trả về là "HocVien" thì sửa case này thành "HocVien")
                    return RedirectToAction("Dashboard", "Student");

                case "Lecturer":
                    // Chuyển đến: Areas/Lecturer/Controllers/HomeController.cs -> Index
                    // (Nếu API trả về là "GiangVien" thì sửa case này thành "GiangVien")
                    return RedirectToAction("Index", "Home");

                default:
                    // Trường hợp không xác định hoặc user thường -> Về trang chủ chung
                    return RedirectToAction("Login", "Login", new { area = "" });
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ============================================================
        // PHẦN MỚI THÊM: QUÊN MẬT KHẨU & RESET PASSWORD
        // ============================================================

        // --- QUÊN MẬT KHẨU ---
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            // SỬA DÒNG NÀY: Trỏ đúng về thư mục Account
            return View("~/Views/Account/ForgotPassword.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                // SỬA DÒNG NÀY
                return View("~/Views/Account/ForgotPassword.cshtml", model);

            var token = await _authApiService.ForgotPasswordAsync(model.Email);

            if (token != null)
            {
                return RedirectToAction("ResetPassword", new { token = token });
            }

            ModelState.AddModelError("", "Email không tồn tại hoặc lỗi hệ thống.");
            // SỬA DÒNG NÀY
            return View("~/Views/Account/ForgotPassword.cshtml", model);
        }

        // --- ĐẶT LẠI MẬT KHẨU ---
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var model = new ResetPasswordViewModel { Token = token };
            // SỬA DÒNG NÀY
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                // SỬA DÒNG NÀY
                return View("~/Views/Account/ResetPassword.cshtml", model);

            var result = await _authApiService.ResetPasswordAsync(model);

            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đổi mật khẩu thất bại.");
            // SỬA DÒNG NÀY
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }
    }
}