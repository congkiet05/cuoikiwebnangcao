using WebQuanLiKhoaHoc_MVC.Models.HocVien;

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class HocVien_XemDiemTBService
    {
        private readonly HttpClient _httpClient;
        public HocVien_XemDiemTBService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7274/");
        }
        public async Task<List<HocVien_XemDiemTBModels>> XemDiemTrungBinh(string MaHocVien)
        {
            var ketqua = await _httpClient.GetFromJsonAsync<List<HocVien_XemDiemTBModels>>($"api/HocVien/XemDiemTrungBinh/{MaHocVien}");
            return ketqua!;
        }

    }
}
