namespace WebQuanLiKhoaHocApi.Dtos
{
    public class HocVien_XemDiemMon
    {
        public string? StudentNumber { get; set; }
        public string? FullName { get; set; }
        public string? ClassCode { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public string? AssignmentTitle { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }

        public double? Grade { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
