namespace WebQuanLiKhoaHoc_MVC.Models.DTOs
{
    public class LecturerDto
    {
        public int LecturerId { get; set; }
        public string StaffNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Department { get; set; }

        // Các trường lấy từ bảng User qua API
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? NewPassword { get; set; }
    }
}