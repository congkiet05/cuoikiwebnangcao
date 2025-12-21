using WebQuanLiKhoaHoc_MVC.Models.HocVien;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class HocVien_XemDiemService
    {
        private readonly HttpClient _httpClient;
        public HocVien_XemDiemService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7274/");
        }
        public async Task<List<HocVien_XemDiemTBModels>> XemDiemTrungBinh(string MaHocVien)
        {
            var ketqua = await _httpClient.GetFromJsonAsync<List<HocVien_XemDiemTBModels>>($"api/HocVien/XemDiemTrungBinh/{MaHocVien}");
            return ketqua!;
        }
        public async Task<List<HocVien_XemDiemMonModels>> XemDiemMonHoc(string MaHocVien, string MaHocPhan)
        {
            var ketqua = await _httpClient.GetFromJsonAsync<List<HocVien_XemDiemMonModels>>($"api/HocVien/XemDiemMon/{MaHocVien}?MaHocPhan={MaHocPhan}");
            return ketqua!;
        }
    }
}