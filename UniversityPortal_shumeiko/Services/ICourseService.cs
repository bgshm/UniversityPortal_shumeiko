using UniversityPortal_shumeiko.Models;

namespace UniversityPortal_shumeiko.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course?> GetCourseByIdForUpdateAsync(int id);
        Task CreateCourseAsync(Course сourse);
        Task UpdateCourseAsync(Course сourse);
        Task DeleteCourseAsync(int id);
        Task<bool> CourseExistsAsync(int id);
    }
}
