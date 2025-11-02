namespace UniversityPortal_shumeiko.Models.ViewModels
{
    // Допоміжний клас для чекбоксу
    public class AssignedCourseData
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public bool Assigned { get; set; }
    }

    public class StudentEditViewModel
    {
        public Student Student { get; set; }
        public List<AssignedCourseData> AssignedCourses { get; set; }
    }
}
