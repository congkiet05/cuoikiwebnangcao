using System.Text.Json;
using System.Text.Json.Serialization;
using WebQuanLiKhoaHoc_MVC.Interface;
using WebQuanLiKhoaHoc_MVC.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class ScheduleApiService : IScheduleApiService
    {
        private readonly HttpClient _httpClient;

        //Khai báo static readonly để tránh khởi tạo lặp lại(Fix lỗi cache options)
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ScheduleApiService(IHttpClientFactory httpClientFactory)
        {
            // Sử dụng HttpClient đã cấu hình tên trong Program.cs
            _httpClient = httpClientFactory.CreateClient("ApiSchedule");
        }
        public async Task<List<StudentScheduleViewModel>> GetStudentScheduleAsync()
        {
            var response = await _httpClient.GetAsync("Schedule/MySchedule");

            if (response.IsSuccessStatusCode)
            {
               var content = await response.Content.ReadAsStringAsync();
                // không phân biệt chữ hoa , thường khi deserialize
                var options  = new JsonSerializerOptions {  PropertyNameCaseInsensitive = true  };
                // Deserialize dữ liệu JSON (từ HocVienScheduleDto) sang ScheduleViewModel
                var schedule = JsonSerializer.Deserialize<List<StudentScheduleViewModel>>(content, _jsonOptions);

                return schedule ?? new List<StudentScheduleViewModel>();
            }
            // Trả về danh sách rỗng nếu có lỗi
            return new List<StudentScheduleViewModel>();
        }
    }
}
