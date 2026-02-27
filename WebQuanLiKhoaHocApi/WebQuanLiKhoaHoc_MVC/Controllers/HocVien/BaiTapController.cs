using Microsoft.AspNetCore.Mvc;
using WebQuanLiKhoaHoc_MVC.Service;

namespace WebQuanLiKhoaHoc_MVC.Controllers.HocVien
{
    public class BaiTapController : Controller
    {
        private readonly BaiTapService _baiTapService;
        public BaiTapController(BaiTapService baiTapService)
        {
            _baiTapService = baiTapService;
        }
        public async Task<IActionResult> ChiTiet(int id)
        {
            // 1. Gọi Service lấy dữ liệu
            var baiTap = await _baiTapService.LayChiTietBaiTap(id);

            // 2. Kiểm tra nếu không tìm thấy
            if (baiTap == null)
            {
                // Có thể chuyển hướng về trang lỗi hoặc trang chủ
                ViewBag.Error("Bạn chưa nhập đáp án của bài tập");
                //return RedirectToAction("Index", "Home");
            }

            // 3. Trả về View kèm dữ liệu
            return View("~/Views/Student/ChiTietBaiTap.cshtml", baiTap);
        }
        [HttpPost]
        public async Task<IActionResult> NopBaiSubmit(int AssignmentId, IFormFile fileBaiLam, string GhiChu)
        {
            if (fileBaiLam == null || fileBaiLam.Length == 0)
            {
                return BadRequest("Vui lòng chọn file bài làm.");
            }

            // 1. Giả lập mã SV (Sau này lấy từ Session)
            string currentStudentNumber = "SV001";

            // 2. Xử lý lưu file (Uploads)
            var fileName = $"{currentStudentNumber}_{AssignmentId}_{DateTime.Now.Ticks}_{fileBaiLam.FileName}";
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileBaiLam.CopyToAsync(stream);
            }

            // 3. Gọi Service
            // Lưu ý: Đường dẫn lưu vào DB nên là đường dẫn tương đối
            string relativePath = $"/uploads/{fileName}";

            // GhiChu vẫn truyền vào nhưng sẽ bị lờ đi ở tầng dưới do DB thiếu cột
            var ketQua = await _baiTapService.NopBai(AssignmentId, currentStudentNumber, relativePath, GhiChu);

            if (ketQua)
            {
                ViewBag.Error = "Nộp Bài Thành Công , Chúc Mừng Bạn ";
                // Để tránh lỗi Model null khi return View, bạn nên redirect hoặc load lại Model
                return RedirectToAction("XemDiemMon","XemDiem");
            }
            else
            {
                return Content("❌ Lỗi: Không thể nộp bài tập");
            }
        }
    }
}
