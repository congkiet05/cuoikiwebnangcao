namespace WebQuanLiKhoaHoc_MVC.Models
{
    public class ScheduleViewModel
    {
        public string TenKhoaHoc { get; set; }
        public string TenGiangVien { get; set; }
        public string MaLopHoc { get; set; }
        public byte NgayTrongTuan { get; set; }
        public TimeOnly ThoiGianBatDau { get; set; }
        public TimeOnly ThoiGianKetThuc { get; set; }
        public string PhongHoc { get; set; }

        public string TenNgayTrongTuan => NgayTrongTuan switch
        {
            1 => "Thứ Hai",
            2 => "Thứ Ba",
            3 => "Thứ Tư",
            4 => "Thứ Năm",
            5 => "Thứ Sáu",
            6 => "Thứ Bảy",
            7 => "Chủ Nhật",
            _ => "Không rõ"
        };
    }

}
