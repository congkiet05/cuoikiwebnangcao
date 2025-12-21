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
    }
}
