
﻿using Microsoft.AspNetCore.Mvc;
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
            int lecturerId = 4;
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
            request.AuthorId = 4;
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
            int lecturerId = 4;

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
            dto.AuthorId = 4; 

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
        public async Task<IActionResult> Assignment()
        {
            int lecturerId = 4; 
            await LoadCommonData(lecturerId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostAssignment(AssignmentDto request)
        {
            int lecturerId = 4;
            request.CreatedAt = DateTime.Now;

            try
            {
               
                var response = await _client.PostAsJsonAsync("Assignments", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Giao bài tập thành công!";
                    return RedirectToAction("Assignment");
                }

                var errorMsg = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = "Lỗi server: " + errorMsg;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi kết nối: " + ex.Message;
            }

            // Load lại dữ liệu để hiển thị lại Form khi có lỗi
            await LoadCommonData(lecturerId);
            return View("Assignment");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAssignment(AssignmentDto dto)
        {
            try
            {
                // Gọi API PUT (Nhớ kiểm tra route Assignments/{id})
                var response = await _client.PutAsJsonAsync($"Assignments/{dto.AssignmentId}", dto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật bài tập thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật bài tập.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
            }

            return RedirectToAction("Assignment");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            try
            {
                var response = await _client.DeleteAsync($"Assignments/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa bài tập thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa bài tập này.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
            }
            return RedirectToAction("Assignment");
        }


        private async Task LoadCommonData(int lecturerId)
        {
            try
            {
                // 1. Lấy danh sách lớp
                var classes = await _client.GetFromJsonAsync<List<ClassDto>>($"Classes/by-lecturer/{lecturerId}");
                ViewBag.TargetClassList = classes?.Select(c => new SelectListItem
                {
                    Value = c.ClassId.ToString(),
                    Text = $"{c.ClassCode} - {c.Semester}"
                }).ToList() ?? new List<SelectListItem>();

                // 2. Lấy danh sách bài tập (SỬA ĐOẠN NÀY)
                var response = await _client.GetAsync($"Assignments/by-lecturer/{lecturerId}");

                if (response.IsSuccessStatusCode)
                {
                    // Cấu hình để không phân biệt chữ hoa/thường khi map JSON
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var assignments = await response.Content.ReadFromJsonAsync<List<AssignmentDto>>(options);
                    ViewBag.PostedAssignments = assignments ?? new List<AssignmentDto>();
                }
                else
                {
                    // Nếu API báo lỗi (404, 500...), log lỗi ra để kiểm tra
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"API Error: {response.StatusCode} - {errorContent}";
                    ViewBag.PostedAssignments = new List<AssignmentDto>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.PostedAssignments = new List<AssignmentDto>();
                ViewBag.ErrorMessage = "Lỗi kết nối hoặc Mapping dữ liệu: " + ex.Message;
            }
        }
        // GET: Lecturer/Profile
        public async Task<IActionResult> Profile()
        {
            int lecturerId = 4;
            // Thêm "api/" vào trước Lecturers nếu BaseAddress chưa có
            var response = await _client.GetAsync($"Lecturers/{lecturerId}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var model = await response.Content.ReadFromJsonAsync<LecturerDto>();
                    return View(model);
                }
                catch (Exception ex)
                {
                    return Content("Lỗi đọc dữ liệu JSON: " + ex.Message);
                }
            }

            // Nếu lỗi, hiện mã lỗi thay vì chuyển trang
            var errorBody = await response.Content.ReadAsStringAsync();
            return Content($"API bị lỗi: {response.StatusCode}. Chi tiết: {errorBody}");
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Tăng cường bảo mật cho Form
        public async Task<IActionResult> UpdateProfile(LecturerDto model)
        {
            // 1. Kiểm tra dữ liệu đầu vào cơ bản (nếu cần)
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            try
            {
             
                var response = await _client.PutAsJsonAsync($"Lecturers/{model.LecturerId}", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    // Đọc lỗi chi tiết từ API nếu có (ví dụ lỗi 400, 500)
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Lỗi cập nhật: {response.StatusCode}";
                    return RedirectToAction("Profile");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi kết nối hệ thống: " + ex.Message;
                return RedirectToAction("Profile");
            }
        }
    }
}
