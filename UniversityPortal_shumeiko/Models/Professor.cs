
namespace UniversityPortal_shumeiko.Models
{
    public class Professor
    {
        public int Id { get; set; }



        public string Name { get; set; }


        public string? Specialization { get; set; }

        public string? ProfilePictureUrl { get; set; }

        // Навігаційна властивість для зв'язку "один-до-багатьох"
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
