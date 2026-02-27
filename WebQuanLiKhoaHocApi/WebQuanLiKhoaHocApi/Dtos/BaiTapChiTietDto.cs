namespace WebQuanLiKhoaHocApi.Dtos
{
    public class BaiTapChiTietDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // Nội dung đề bài
        public DateTime? DueDate { get; set; }
        public string TenMon { get; set; }
    }
}
