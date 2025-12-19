using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebQuanLiKhoaHoc_MVC.Models.DTOs;

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class LecturerController : Controller
    {
        private readonly HttpClient _client;

        public LecturerController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("ApiClient");
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public async Task<IActionResult> Announcement()
        {
            int lecturerId = 3;
            ViewBag.CurrentLecturerId = lecturerId;

            try
            {
                var lecturerClasses = await _client.GetFromJsonAsync<List<ClassDto>>(
                    $"Classes/by-lecturer/{lecturerId}" // Endpoint đã được Include(Course)
                );

                var classes = lecturerClasses ?? new List<ClassDto>();

                // 2. CHUYỂN ĐỔI sang SELECTLISTITEM
                var classSelectList = classes
                    .Select(c => new SelectListItem
                    {
                        Value = c.ClassId.ToString(),
                        // Sử dụng thuộc tính tiện ích (CourseCode, CourseName)
                        Text = $"{c.ClassCode} - {c.CourseName ?? "No Name"} ({c.Semester})"
                    })
                    .ToList();
                 var responseAnnouncements = await _client.GetAsync($"Announcements/by-lecturer/{lecturerId}");
                if (responseAnnouncements.IsSuccessStatusCode)
                {
                    var announcements = await responseAnnouncements.Content.ReadFromJsonAsync<List<AnnouncementDto>>();
                    // Đưa vào ViewBag để hiển thị ở cột bên phải
                    ViewBag.PostedAnnouncements = announcements ?? new List<AnnouncementDto>();
                }
                else
                {
                    ViewBag.PostedAnnouncements = new List<AnnouncementDto>();
                }

                // 3. Đưa danh sách đã chuyển đổi vào ViewBag
                ViewBag.TargetClassList = classSelectList;
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage = "Không thể kết nối đến máy chủ API.";
                ViewBag.TargetClassList = new List<SelectListItem>();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostAnnouncement(AnnouncementDto request)
        {
            request.AuthorId = 3;
            request.CreatedAt = DateTime.Now;

            try
            {
                var response = await _client.PostAsJsonAsync("Announcements", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đã gửi thông báo thành công!";
                    return RedirectToAction("Announcement");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Lỗi từ máy chủ ({(int)response.StatusCode}): {errorContent}";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối: " + ex.Message;
            }
            return View("Announcement");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var response = await _client.DeleteAsync($"Announcements/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đã xoá thông báo thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Xoá thông báo thất bại.";
            }

            return RedirectToAction("Announcement");
        }
        [HttpGet]
        public async Task<IActionResult> EditAnnouncement(int id)
        {
            int lecturerId = 3;

            var response = await _client.GetAsync($"Announcements/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<AnnouncementDto>();

            // Load class list
            var classes = await _client.GetFromJsonAsync<List<ClassDto>>(
                $"Classes/by-lecturer/{lecturerId}") ?? new();

            ViewBag.TargetClassList = classes.Select(c => new SelectListItem
            {
                Value = c.ClassId.ToString(),
                Text = $"{c.ClassCode} - {c.CourseName} ({c.Semester})"
            }).ToList();

            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> EditAnnouncement(AnnouncementDto dto)
        {
            dto.AuthorId = 3; // sau này lấy từ token
                              // KHÔNG set CreatedAt

            var response = await _client.PutAsJsonAsync(
                $"Announcements/{dto.AnnouncementId}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thông báo thành công!";
                return RedirectToAction("Announcement");
            }

            TempData["ErrorMessage"] = await response.Content.ReadAsStringAsync();
            return RedirectToAction("EditAnnouncement", new { id = dto.AnnouncementId });
        }


        public IActionResult Notification()
        {
            return View();
        }
        public IActionResult Message()
        {
            return View();
        }
        public IActionResult Timetable()
        {
            return View();
        }
        public IActionResult Assignment()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
    }
}
