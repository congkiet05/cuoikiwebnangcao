using System;

namespace WebQuanLiKhoaHoc_MVC.Models.HocVien
{
    
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}