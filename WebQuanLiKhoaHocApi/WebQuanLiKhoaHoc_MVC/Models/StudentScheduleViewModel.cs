using System.Text.Json.Serialization;

namespace WebQuanLiKhoaHoc_MVC.Models
{
    public class StudentScheduleViewModel
    {
        [JsonPropertyName("scheduleId")]
        public int ScheduleId { get; set; }

        [JsonPropertyName("maLopHoc")]
        public string MaLopHoc { get; set; }

        [JsonPropertyName("tenKhoaHoc")]
        public string TenKhoaHoc { get; set; }

        [JsonPropertyName("maKhoaHoc")]
        public string MaKhoaHoc { get; set; }

        [JsonPropertyName("credit")]
        public int Credit { get; set; }

        // --- BỔ SUNG DÒNG NÀY ĐỂ SỬA LỖI ---
        [JsonPropertyName("hocKy")]
        public string HocKy { get; set; }
        // -----------------------------------

        [JsonPropertyName("tenGiangVien")]
        public string TenGiangVien { get; set; }

        [JsonPropertyName("ngayTrongTuan")]
        public int NgayTrongTuan { get; set; }

        [JsonPropertyName("thoiGianBatDau")]
        public TimeOnly ThoiGianBatDau { get; set; }

        [JsonPropertyName("thoiGianKetThuc")]
        public TimeOnly ThoiGianKetThuc { get; set; }

        [JsonPropertyName("phongHoc")]
        public string PhongHoc { get; set; }
    }
}