using WebQuanLiKhoaHocApi.Dtos;

namespace WebQuanLiKhoaHocApi.Services
{
    public interface IScheduleService
    {
        Task<IEnumerable<HocVienScheduleDto>> GetStudentScheduleAsync(int studentId);
    }
}
