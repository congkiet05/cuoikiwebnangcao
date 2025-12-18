using System.Text.Json;
using System.Text.Json.Serialization;
using WebQuanLiKhoaHoc_MVC.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class ScheduleApiService 
    {
        private readonly HttpClient _httpClient;

        //Khai báo static readonly để tránh khởi tạo lặp lại(Fix lỗi cache options)
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

       
    }
}
