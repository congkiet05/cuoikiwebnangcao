using WebQuanLiKhoaHocApi.Entities;

namespace WebQuanLiKhoaHoc_MVC.Models.HocVien
{
    public class ClassEnrollmentViewModel
    {
        public int ClassId { get; set; }
        public string? ClassCode { get; set; }
        public string? CourseName { get; set; }
        public string? LecturerName { get; set; }
        public int Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public List<ClassSchedule> Schedules { get; set; } = new();
    }
}