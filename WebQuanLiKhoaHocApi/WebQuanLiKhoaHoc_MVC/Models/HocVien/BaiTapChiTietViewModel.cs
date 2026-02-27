using System;

namespace WebQuanLiKhoaHoc_MVC.Models.BaiTap
{
    public class BaiTapChiTietViewModel
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } // Nội dung đề bài (đoạn văn)
        public DateTime? DueDate { get; set; }
        public string TenMon { get; set; }
    }
}