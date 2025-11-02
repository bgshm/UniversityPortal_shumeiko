namespace UniversityPortal_shumeiko.Models.ViewModels
{
    public class ProfessorCreateViewModel
    {
        public string Name { get; set; }

        public string? Specialization { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }
    }
}
