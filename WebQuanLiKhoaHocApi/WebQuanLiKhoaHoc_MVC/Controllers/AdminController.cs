using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebQuanLiKhoaHoc_MVC.Models;
using WebQuanLiKhoaHoc_MVC.Service;
using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _api;

        public AdminController(ApiService api)
        {
            _api = api;
        }

        // --- ĐÃ XÓA DÒNG Dashboard() => View() Ở ĐÂY ĐỂ TRÁNH TRÙNG LẶP ---

        // ================== 1. QUẢN LÝ USER ==================
        public async Task<IActionResult> User()
        {
            var data = await _api.GetAllAsync<UserViewModel>("users");
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] UserViewModel model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ");

            var payload = new
            {
                Id = model.Id,
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                RoleId = model.RoleId,
                TrangThai = model.TrangThai,
                HoTen = model.HoTen,
                BoMon = model.BoMon
            };

            bool result;

            if (model.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    return BadRequest("Mật khẩu không được để trống");

                result = await _api.CreateAsync("users", payload);
            }
            else
            {
                result = await _api.UpdateAsync($"users/{model.Id}", payload);
            }

            return result ? Ok() : BadRequest("Không thể lưu dữ liệu");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _api.DeleteAsync($"users/{id}");
            return result ? Ok() : BadRequest();
        }

        // ================== 2. QUẢN LÝ KHÓA HỌC ==================
        public async Task<IActionResult> KhoaHoc()
        {
            var data = await _api.GetAllAsync<KhoaHocViewModel>("courses");
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> SaveCourse([FromBody] KhoaHocViewModel model)
        {
            if (model == null)
                return BadRequest("Dữ liệu không hợp lệ");

            bool result;

            if (model.Id == 0)
            {
                // 👉 THÊM MỚI
                result = await _api.CreateAsync("courses", model);
            }
            else
            {
                // 👉 CẬP NHẬT
                result = await _api.CreateAsync("courses", model);
                // API courses POST xử lý cả thêm + sửa dựa vào Id
            }

            return result ? Ok() : BadRequest("Không thể lưu khóa học");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id) // 👈 Phải là int
        {
            // Gọi API xóa theo ID
            var result = await _api.DeleteAsync($"courses/{id}");
            return result ? Ok() : BadRequest();
        }

        // ================== 3. QUẢN LÝ LỊCH HỌC ==================
        // HIỂN THỊ
        [HttpGet("LichHoc")]
        public async Task<IActionResult> LichHoc()
        {
            var data = await _api.GetAllAsync<ScheduleViewModel>("schedules");
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleViewModel model)
        {
            var ok = await _api.CreateAsync("schedules", model);
            return ok ? Ok() : BadRequest("Không thể thêm lịch");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] ScheduleViewModel model)
        {
            var ok = await _api.UpdateAsync($"schedules/{id}", model);
            return ok ? Ok() : BadRequest("Không thể cập nhật lịch");
        }
        // XÓA
        [HttpPost]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            return await _api.DeleteAsync($"schedules/{id}") ? Ok() : BadRequest();
        }


        // ================== 4. DASHBOARD (ĐÃ SỬA LỖI) ==================
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // SỬA LỖI: Đổi GetData -> GetAllAsync
                var users = await _api.GetAllAsync<UserViewModel>("users");
                var courses = await _api.GetAllAsync<KhoaHocViewModel>("courses");

                // 1. Số liệu tổng quan
                ViewBag.TotalUsers = users.Count;
                ViewBag.TotalCourses = courses.Count;
                ViewBag.ActiveUsers = users.Count(u => u.TrangThai == 2);

                // Tính doanh thu giả định (xử lý null nếu courses rỗng)
                ViewBag.Revenue = courses.Any() ? courses.Sum(c => c.TinChi * 500000) : 0;

                // 2. Dữ liệu Biểu đồ đường
                ViewBag.ChartLabels = new[] { "T1", "T2", "T3", "T4", "T5", "T6" };
                ViewBag.ChartData = new[] { 10, 25, 40, 35, 60, 85 };

                // 3. Dữ liệu Biểu đồ tròn
                var cnttCount = users.Count(u => u.BoMon == "CNTT");
                var ktCount = users.Count(u => u.BoMon == "KinhTe");
                ViewBag.PieLabels = new[] { "CNTT", "Kinh Tế", "Khác" };
                ViewBag.PieData = new[] { cnttCount, ktCount, users.Count - cnttCount - ktCount };

                // 4. Danh sách sinh viên mới nhất
                ViewBag.NewUsers = users.OrderByDescending(u => u.Id).Take(5).ToList();

                return View("~/Views/Admin/Dashboard.cshtml");
            }
            catch
            {
                // Khởi tạo giá trị mặc định để View không bị lỗi Null
                ViewBag.TotalUsers = 0;
                ViewBag.TotalCourses = 0;
                ViewBag.ActiveUsers = 0;
                ViewBag.Revenue = 0;
                ViewBag.NewUsers = new List<UserViewModel>();

                return View("~/Views/Admin/Dashboard.cshtml");
            }
        }
        // ================== 5. QUẢN LÝ THÔNG BÁO ==================

        // Hiển thị danh sách thông báo
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Announcement()
        {
            var announcements = await _api.GetAllAsync<AnnouncementAdminDto>("announcement");
            var viewModel = announcements.Select(a => new AnnouncementAdminViewModel
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Body = a.Body,
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
                CreatedAt = a.CreatedAt,
                IsGlobal = a.IsGlobal,
                TargetClassId = a.TargetClassId
            }).OrderByDescending(a => a.CreatedAt).ToList();

            return View(viewModel);
        }


        // Form tạo mới thông báo
        // [Authorize(Roles = "Admin")]
        public IActionResult CreateAnnouncement()
        {
            return View("~/Views/Admin/CreateAnnouncement.cshtml");
        }

        // Thêm thông báo mới
        [HttpPost]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] AnnouncementAdminViewModel model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ");

            // Tạo payload gửi API
            var payload = new
            {
                Title = model.Title,
                Body = model.Body,
                AuthorId = 1, // ID admin cố định để thử nghiệm
                IsGlobal = true,
                TargetClassId = model.TargetClassId
            };

            var ok = await _api.CreateAsync("announcement", payload);

            if (!ok) return BadRequest("Không thể tạo thông báo");

            // Trả về thông báo vừa tạo để hiển thị thử mà không reload page
            var newAnnouncement = new AnnouncementAdminViewModel
            {
                AnnouncementId = new Random().Next(1000, 9999), // tạm giả lập ID
                Title = model.Title,
                Body = model.Body,
                AuthorId = 1,
                AuthorName = "Admin",
                CreatedAt = DateTime.Now,
                IsGlobal = true
            };

            return Ok(newAnnouncement);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditAnnouncement(int id)
        {
            var announcements = await _api.GetAllAsync<AnnouncementAdminViewModel>("announcement");
            var announcement = announcements.FirstOrDefault(a => a.AnnouncementId == id);
            if (announcement == null) return NotFound();
            return View("~/Views/Admin/EditAnnouncement.cshtml", announcement);
        }

        // Cập nhật thông báo
        [HttpPost]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementAdminViewModel model)
        {
            // Validate dữ liệu đầu vào
            if (model == null || id != model.AnnouncementId)
            {
                return BadRequest("Dữ liệu không hợp lệ hoặc ID không khớp.");
            }

            // Tạo payload đúng chuẩn API yêu cầu (AnnouncementAdminDto)
            // QUAN TRỌNG: Phải có AnnouncementId để API check (id == model.AnnouncementId)
            var payload = new
            {
                AnnouncementId = model.AnnouncementId, // <--- Bắt buộc phải có dòng này
                Title = model.Title,
                Body = model.Body,
                IsGlobal = model.IsGlobal,
                TargetClassId = model.TargetClassId,
                AuthorId = model.AuthorId
            };

            // Gọi API qua phương thức PUT (UpdateAsync)
            // Đảm bảo đường dẫn "announcement/{id}" khớp với route API [HttpPut("announcement/{id}")]
            var success = await _api.UpdateAsync($"announcement/{id}", payload);

            if (success)
            {
                // Vì API Update trả về object DTO mới nhất (theo code bạn gửi),
                // nhưng hàm _api.UpdateAsync thường chỉ trả về bool (true/false).
                // Nên ta trả về chính cái model vừa gửi lên để JS cập nhật giao diện ngay lập tức.
                // (Hoặc nếu _api của bạn trả về data thì tốt hơn, nhưng trả model là giải pháp an toàn).
                return Ok(model);
            }

            return BadRequest("Lỗi khi cập nhật (Kiểm tra lại Log Server API)");
        }
        // Xóa thông báo
        [HttpPost]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            // Gọi API qua phương thức DELETE (DeleteAsync)
            // Đảm bảo đường dẫn "announcement/{id}" khớp với route API [HttpDelete("announcement/{id}")]
            var success = await _api.DeleteAsync($"announcement/{id}");

            if (success)
            {
                return Ok();
            }

            return BadRequest("Không thể xóa thông báo (Có thể ID không tồn tại)");
        }
    }
}