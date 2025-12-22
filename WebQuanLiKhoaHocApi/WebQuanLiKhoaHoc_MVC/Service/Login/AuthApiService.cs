using Newtonsoft.Json;
using System.Text;
using WebQuanLiKhoaHoc_MVC.Models.Login;

namespace WebQuanLiKhoaHoc_MVC.Service.Login
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7274/api/Auth/login";
        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<LoginResponseModel?> LoginAsync(string username, string password)
        {
            var loginData = new LoginRequestModel
            {
                Username = username,
                Password = password
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(loginData),
                Encoding.UTF8,
                "application/json");

            // Gọi sang API
            var response = await _httpClient.PostAsync(_baseUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return null; // Đăng nhập thất bại
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponseModel>(responseString);
        }
        public async Task<string> ForgotPasswordAsync(string email)
        {
            // LƯU Ý: Thay số 7274 bằng cổng (port) chính xác mà API của bạn đang chạy
            // (Xem trong file Properties/launchSettings.json của Project API)
            string apiUrl = "https://localhost:7274/api/auth/forgot-password";

            var payload = new { Email = email };

            // Sửa dòng này: Truyền apiUrl đầy đủ vào
            var response = await _httpClient.PostAsJsonAsync(apiUrl, payload);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                try { return result.GetProperty("token").GetString(); } catch { return "check-email"; }
            }
            return null;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            // LƯU Ý: Thay số 7274 bằng cổng (port) chính xác
            string apiUrl = "https://localhost:7274/api/auth/reset-password";

            // Sửa dòng này: Truyền apiUrl đầy đủ vào
            var response = await _httpClient.PostAsJsonAsync(apiUrl, model);

            return response.IsSuccessStatusCode;
        }
    }
}
