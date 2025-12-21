namespace WebQuanLiKhoaHocApi.DTOs
{
    public class NotificationDTO
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } // Trạng thái đã xem
        public string Type { get; set; } // "Toàn trường" hoặc "Lớp học"
    }
}