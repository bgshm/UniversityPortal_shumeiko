namespace UniversityPortal_shumeiko.Models.ViewModels
{
    public class CourseCreateViewModel
    {
        public string Title { get; set; }
        public int Credits { get; set; }
        public string? Department { get; set; }
        public int ProfessorId { get; set; }
    }
}
