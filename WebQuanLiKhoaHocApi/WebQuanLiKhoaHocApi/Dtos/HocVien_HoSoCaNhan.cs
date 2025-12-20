namespace WebQuanLiKhoaHocApi.Dtos
{
    public class HocVien_HoSoCaNhan
    {
        public string MaHocVien { get; set; }
        public string HoVaTen { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public string NganhHoc { get; set; }
        public int? KhoaNhapHoc { get; set; }
    }
}
