using WebQuanLiKhoaHoc_MVC.Models.HocVien;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class HoSoHocVienService
    {
        private readonly HttpClient _httpClient;
        public HoSoHocVienService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7274/");
        }
        public async Task<HoSoHocVien?> LayHoSoHocVien(string MaHocVien)
        {
            return await _httpClient.GetFromJsonAsync<HoSoHocVien>($"api/HocVien/HoSoHocVien/{MaHocVien}");
        }
    }
}
