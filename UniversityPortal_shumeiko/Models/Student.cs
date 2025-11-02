namespace UniversityPortal_shumeiko.Models
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime EnrollmentDate { get; set; }

        // Навігаційна властивість для зв'язку "багато-до-багатьох"
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
