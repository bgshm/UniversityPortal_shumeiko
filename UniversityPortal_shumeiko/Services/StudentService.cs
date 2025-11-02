using UniversityPortal_shumeiko.Data;
using UniversityPortal_shumeiko.Models;
using Microsoft.EntityFrameworkCore;

namespace UniversityPortal_shumeiko.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students.ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
               .Include(s => s.Enrollments) // Включаємо записи про зарахування
                   .ThenInclude(e => e.Course) // А потім включаємо самі курси
               .AsNoTracking() // Використовуємо для оптимізації, оскільки ми не будемо напряму змінювати цей об'єкт
               .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Student?> GetStudentByIdForUpdateAsync(int id)
        {
            // Цей метод НЕ використовує AsNoTracking(), щоб EF Core міг відстежувати зміни
            return await _context.Students
              .Include(s => s.Enrollments)
              .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateStudentAsync(Student student)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(e => e.Id == id);
        }
    }
}
