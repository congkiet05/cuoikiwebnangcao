using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WebQuanLiKhoaHocApi.DTOs; // Dùng chung DTO hoặc tạo class DTO tương tự trong MVC

namespace WebQuanLiKhoaHoc_MVC.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AnnouncementController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            // Giả lập lấy ID sinh viên từ Session (Thay bằng logic thật của bạn)
            // int studentId = HttpContext.Session.GetInt32("UserId") ?? 0;
            int studentId = 1;

            var data = new List<NotificationDTO>();

            // Gọi API
            var client = _clientFactory.CreateClient();
            // LƯU Ý: Thay đổi PORT cho đúng với project API của bạn
            var response = await client.GetAsync($"https://localhost:7123/api/Notification/get-all-list/{studentId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                data = JsonConvert.DeserializeObject<List<NotificationDTO>>(jsonString);
            }

            return View(data);
        }

        // Action xem chi tiết (nếu muốn chuyển sang trang riêng thay vì modal)
        public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7123/api/Notification/get-detail/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<NotificationDTO>(jsonString);
                return View(item);
            }
            return NotFound();
        }
    }
}