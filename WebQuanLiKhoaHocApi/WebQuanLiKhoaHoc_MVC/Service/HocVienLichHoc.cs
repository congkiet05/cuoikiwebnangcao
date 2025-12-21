using WebQuanLiKhoaHoc_MVC.Models.HocVien;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class HocVienLichHoc
    {
        private readonly HttpClient _httpClient;
        public HocVienLichHoc(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7274/");
        }
        public async Task<List<LichHocModel>> XemLichHoc(string MaHocVien)
        {
            var ketqua = await _httpClient.GetFromJsonAsync<List<LichHocModel>>($"api/HocVien/LichHoc/{MaHocVien}");
            return ketqua!;
        }
    }
}
