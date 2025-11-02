using UniversityPortal_shumeiko.Models;

namespace UniversityPortal_shumeiko.Services
{
    public interface IProfessorService
    {
        Task<IEnumerable<Professor>> GetAllProfessorsAsync();
        Task<Professor?> GetProfessorByIdAsync(int id);
        Task CreateProfessorAsync(Professor professor);
        Task UpdateProfessorAsync(Professor professor);
        Task DeleteProfessorAsync(int id);
        Task<bool> ProfessorExistsAsync(int id);
    }
}
