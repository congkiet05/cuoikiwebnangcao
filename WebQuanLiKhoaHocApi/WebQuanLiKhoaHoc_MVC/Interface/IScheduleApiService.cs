using WebQuanLiKhoaHoc_MVC.Models;

namespace WebQuanLiKhoaHoc_MVC.Interface
{
    public interface IScheduleApiService
    {
        Task<List<StudentScheduleViewModel>> GetStudentScheduleAsync();
    }
}
