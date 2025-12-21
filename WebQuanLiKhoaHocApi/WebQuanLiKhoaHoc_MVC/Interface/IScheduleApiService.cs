using WebQuanLiKhoaHoc_MVC.Models;

namespace WebQuanLiKhoaHoc_MVC.Interface
{
    public interface IScheduleApiService
    {
        // 1. Lấy danh sách lịch học
        Task<List<StudentScheduleViewModel>> GetStudentScheduleAsync();

        // 2. Thêm lịch học mới
        Task<bool> CreateScheduleAsync(StudentScheduleViewModel model);

        // 3. Cập nhật lịch học
        Task<bool> UpdateScheduleAsync(int id, StudentScheduleViewModel model);

        // 4. Xóa lịch học
        Task<bool> DeleteScheduleAsync(int id);
    }
}