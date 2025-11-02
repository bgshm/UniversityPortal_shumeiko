using UniversityPortal_shumeiko.Data;
using UniversityPortal_shumeiko.Models;
using Microsoft.EntityFrameworkCore;

namespace UniversityPortal_shumeiko.Services
{
    public class ProfessorService : IProfessorService
    {
        private readonly ApplicationDbContext _context;

        public ProfessorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Professor>> GetAllProfessorsAsync()
        {
            return await _context.Professors.ToListAsync();
        }

        public async Task<Professor?> GetProfessorByIdAsync(int id)
        {
            return await _context.Professors
               .Include(p => p.Courses) // Жадібне завантаження курсів
               .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateProfessorAsync(Professor professor)
        {
            _context.Add(professor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfessorAsync(Professor professor)
        {
            _context.Update(professor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProfessorAsync(int id)
        {
            var professor = await _context.Professors.FindAsync(id);
            if (professor != null)
            {
                _context.Professors.Remove(professor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ProfessorExistsAsync(int id)
        {
            return await _context.Professors.AnyAsync(e => e.Id == id);
        }
    }
}
