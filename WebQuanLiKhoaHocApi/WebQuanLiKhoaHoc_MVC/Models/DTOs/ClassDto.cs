namespace WebQuanLiKhoaHoc_MVC.Models.DTOs
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassCode { get; set; }
        public string Semester { get; set; }
        public int Capacity { get; set; }
        public int CourseId { get; set; }

        // Mối quan hệ lồng ghép (Nested)
        public CourseDto Course { get; set; }

        // Thuộc tính tiện ích để truy cập an toàn tên môn học
        public string CourseName => Course?.CourseName;
        public string CourseCode => Course?.CourseCode;
    }
}
