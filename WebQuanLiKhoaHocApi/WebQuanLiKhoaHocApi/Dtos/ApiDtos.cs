using System.Text.Json.Serialization;

namespace WebQuanLiKhoaHocApi.Dtos
{
    // 1. DTO cho trang Quản Lý Người Dùng
    public class UserDto
    {
        public int Id { get; set; }

        public string HoTen { get; set; }
        public string Email { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string BoMon { get; set; }
        public int RoleId { get; set; }
        public int TrangThai { get; set; }

        // ===== CHỈ HIỂN THỊ, KHÔNG NHẬN TỪ CLIENT =====
        [JsonIgnore]
        public string ?RoleName { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime? LastLoginAt { get; set; }
    }

    // 2. DTO cho trang Quản Lý Khóa Học (Danh sách lớp)
    public class KhoaHocDto
    {
        public int Id { get; set; }
        public string MaMon { get; set; } // Map với CourseCode
        public string TenMon { get; set; } // Map với CourseName
        public int TinChi { get; set; } = 3; // Mặc định 3 tín chỉ
        public string MoTa { get; set; }
    }

    // 3. DTO cho trang Quản Lý Lớp Học (Lịch học chi tiết)
    public class ScheduleDto
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
    public class AnnouncementAdminDto
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public int? AuthorId { get; set; }         // Lấy từ entity.AuthorId
        public string AuthorName { get; set; } = "Unknown"; // Lấy từ entity.Author.Username
        public DateTime CreatedAt { get; set; }
        public bool IsGlobal { get; set; }
        public int? TargetClassId { get; set; }
    }

}