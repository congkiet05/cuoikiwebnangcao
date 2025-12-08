namespace WebQuanLiKhoaHoc_MVC.Models
{
    public class StudentScheduleViewModel
    {
        public string MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public int Credit { get; set; }
        public string MaLopHoc { get; set; }
        public string HocKy { get; set; }
        public string TenGiangVien { get; set; }
        public int ScheduleId { get; set; }
        public byte NgayTrongTuan { get; set; }
        public TimeOnly ThoiGianBatDau { get; set; }
        public TimeOnly ThoiGianKetThuc { get; set; }
        public string PhongHoc { get; set; }
    }

}
