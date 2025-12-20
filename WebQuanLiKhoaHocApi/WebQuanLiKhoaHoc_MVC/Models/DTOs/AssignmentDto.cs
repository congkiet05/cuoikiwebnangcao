namespace WebQuanLiKhoaHoc_MVC.Models.DTOs
{
    public class AssignmentDto
    {
        public int AssignmentId { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ClassCode { get; set; }

        // --- THÊM 2 TRƯỜNG NÀY ---
        public int SubmissionCount { get; set; } // Đếm số dòng trong bảng Submission
        public int TotalStudents { get; set; }   // Lấy từ trường Capacity của bảng Class
    }
}