using WebQuanLiKhoaHoc_MVC.Models.BaiTap;
using WebQuanLiKhoaHoc_MVC.Models.HocVien; // Nhớ using namespace chứa ViewModel

namespace WebQuanLiKhoaHoc_MVC.Service
{
    public class BaiTapService
    {
        private readonly HttpClient _httpClient;

        public BaiTapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Thiết lập địa chỉ API cứng giống file cũ của bạn
            _httpClient.BaseAddress = new Uri("https://localhost:7274/");
        }

        public async Task<List<BaiTapCanNopViewModel>> LayBaiTapChuaNop(string studentNumber, string classCode)
        {
            // Gọi API gọn gàng như cách bạn làm
            var ketqua = await _httpClient.GetFromJsonAsync<List<BaiTapCanNopViewModel>>($"api/BaiTap/ChuaNop?studentNumber={studentNumber}&classCode={classCode}");
            return ketqua!;
        }
        public async Task<BaiTapChiTietViewModel> LayChiTietBaiTap(int id)
        {
            try
            {
                // Gọi vào API: api/BaiTap/5
                var result = await _httpClient.GetFromJsonAsync<BaiTapChiTietViewModel>($"api/BaiTap/{id}");
                return result;
            }
            catch
            {
                return null; // Trả về null nếu lỗi hoặc không tìm thấy
            }
        }
        public async Task<bool> NopBai(int assignmentId, string studentNumber, string filePath, string notes)
        {
            // Tạo object khớp với NopBaiDto bên API
            var data = new
            {
                AssignmentId = assignmentId,
                StudentNumber = studentNumber,
                FileUrl = filePath // Sửa thành FileUrl
                                   // Notes = notes // Tạm thời không gửi Notes đi vì API không nhận
            };

            var response = await _httpClient.PostAsJsonAsync("api/BaiTap/NopBai", data);
            return response.IsSuccessStatusCode;
        }
    }
}