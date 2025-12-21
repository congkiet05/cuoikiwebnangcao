using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHoc_MVC.Models.HocVien
{
    public class Registration
    {
        public int RegistrationId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Status { get; set; } = "Registered";

        // Navigation properties
        public virtual Student? Student { get; set; }
        public virtual Class? Class { get; set; }
    }
}
