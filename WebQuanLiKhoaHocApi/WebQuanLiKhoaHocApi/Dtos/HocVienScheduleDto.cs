namespace WebQuanLiKhoaHocApi.Dtos
{
    public class HocVienScheduleDto
    {
        // Thông tin Khóa học (Course)
        public string MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public int Credit { get; set; }

        // Thông tin Lớp học (Class)
        public string MaLopHoc { get; set; }
        public string HocKy { get; set; }
        public string TenGiangVien { get; set; } // Cần join qua Lecturer

        // Thông tin Buổi học (ClassSchedule)
        public int ScheduleId { get; set; }
        public byte NgayTrongTuan { get; set; }
        public TimeOnly ThoiGianBatDau { get; set; }
        public TimeOnly ThoiGianKetThuc { get; set; }
        public string PhongHoc { get; set; }
    }
}
