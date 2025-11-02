using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityPortal_shumeiko.Models
{
    public class Course
    {
        public int Id { get; set; }



        public string Title { get; set; }


        public int Credits { get; set; }


        public string? Department { get; set; }

        // Зовнішній ключ для зв'язку з Professor
        public int ProfessorId { get; set; }

        // Навігаційна властивість
        [ForeignKey("ProfessorId")]
        public virtual Professor Professor { get; set; }

        // Навігаційна властивість для зв'язку "багато-до-багатьох"
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
