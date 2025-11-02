using Microsoft.EntityFrameworkCore;
using UniversityPortal_shumeiko.Data;
using UniversityPortal_shumeiko.Models;

namespace UniversityPortal_shumeiko.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;
        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            // Використовуємо Include для завантаження пов'язаних даних про професора
            return await _context.Courses.Include(c => c.Professor).ToListAsync();
        }
        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
               .Include(c => c.Professor) // Професор
               .Include(c => c.Enrollments) // Зарахування
                   .ThenInclude(e => e.Student) // Студенти
               .AsNoTracking()
               .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<Course?> GetCourseByIdForUpdateAsync(int id)
        {
            // Этот метод для Редактирования (Edit). Он НЕ использует AsNoTracking(),
            // потому что нам нужно, чтобы DbContext отслеживал изменения в этом объекте.
            return await _context.Courses
              .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task CreateCourseAsync(Course course)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCourseAsync(Course course)
        {
            _context.Update(course);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> CourseExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(e => e.Id == id);
        }
    }
}
