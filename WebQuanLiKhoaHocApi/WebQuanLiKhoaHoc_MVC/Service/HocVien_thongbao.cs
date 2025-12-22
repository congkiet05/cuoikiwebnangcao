using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class HocVien_thongbao
    {
        private readonly HttpClient _httpClient;

        public HocVien_thongbao(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Port 7274
            _httpClient.BaseAddress = new System.Uri("https://localhost:7274/api/");
        }

        // CHÚ Ý: Tôi viết rõ namespace đầy đủ ở đây để tránh nhầm lẫn
        public async Task<List<WebQuanLiKhoaHoc_MVC.Models.HocVien.Announcement>> GetAnnouncementsAsync()
        {
            var announcements = new List<WebQuanLiKhoaHoc_MVC.Models.HocVien.Announcement>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("Announcements");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    // Ép kiểu tường minh về Model của MVC
                    announcements = JsonConvert.DeserializeObject<List<WebQuanLiKhoaHoc_MVC.Models.HocVien.Announcement>>(data);
                }
            }
            catch (System.Exception)
            {
                // Bỏ qua lỗi
            }

            return announcements;
        }
    }
}