namespace WebQuanLiKhoaHoc_MVC.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        // ===== GỬI LÊN API =====
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int TrangThai { get; set; }
        public string HoTen { get; set; }
        public string BoMon { get; set; }

        // ===== CHỈ HIỂN THỊ – KHÔNG GỬI =====
        public string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class KhoaHocViewModel
    {
        public int Id { get; set; }
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public int TinChi { get; set; }
        public string MoTa { get; set; }
    }

    public class ScheduleViewModel
    {
        public int ScheduleId { get; set; }

        // ===== CHỈ HIỂN THỊ (GET) =====
        public string? MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public int Credit { get; set; }
        public string? HocKy { get; set; }

        // ===== NHẬN TỪ MVC =====
        public string MaLopHoc { get; set; } = null!;
        public string? TenGiangVien { get; set; }
        public byte NgayTrongTuan { get; set; }

        public TimeOnly ThoiGianBatDau { get; set; }
        public TimeOnly ThoiGianKetThuc { get; set; }
        public string? PhongHoc { get; set; }
    }
    public class AnnouncementAdminViewModel
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsGlobal { get; set; }
        public int? TargetClassId { get; set; }
    }
    //public class NotificationAdminViewModel
    //{
    //    public int AnnouncementId { get; set; }
    //    public string Title { get; set; } = "";
    //    public string Body { get; set; } = "";
    //    public int AuthorId { get; set; }
    //    public string AuthorName { get; set; } = "";
    //    public DateTime CreatedAt { get; set; }
    //    public bool IsRead { get; set; } = false;
    //}
}