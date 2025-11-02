namespace UniversityPortal_shumeiko.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public int EnrollmentId { get; set; }

        // Зовнішні ключі
        public int CourseId { get; set; }
        public int StudentId { get; set; }


        public Grade? Grade { get; set; }

        // Навігаційні властивості
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
