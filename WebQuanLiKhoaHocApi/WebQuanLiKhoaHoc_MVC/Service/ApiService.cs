using System.Text;
using System.Text.Json;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl = "https://localhost:7274/api/AdminApi/"; // ⚠️ Kiểm tra lại cổng API của bạn

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("ApiGeneric"); // Đăng ký tên này trong Program.cs
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _jsonOptions.Converters.Add(new TimeOnlyConverter());
        }

        // 1. GET (Lấy danh sách)
        public async Task<List<T>> GetAllAsync<T>(string endpoint)
        {
            try
            {
                var response = await _http.GetAsync(_baseUrl + endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(json)) return new List<T>();
                    return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
                }
            }
            catch { }
            return new List<T>();
        }

        // 2. POST (Thêm mới)
        public async Task<bool> CreateAsync<T>(string endpoint, T model)
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 👉 GỌI ĐÚNG API
            var response = await _http.PostAsync(_baseUrl + endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Error: " + error);
            }

            return response.IsSuccessStatusCode;
        }


        // 3. PUT (Cập nhật)
        public async Task<bool> UpdateAsync<T>(string endpoint, T model)
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PutAsync(_baseUrl + endpoint, content);
            return response.IsSuccessStatusCode;
        }

        // 4. DELETE (Xóa)
        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await _http.DeleteAsync(_baseUrl + endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}