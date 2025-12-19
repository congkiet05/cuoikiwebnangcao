namespace WebQuanLiKhoaHoc_MVC.Models.DTOs
{
    public class AnnouncementDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int AuthorId { get; set; }
        public int? TargetClassId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
