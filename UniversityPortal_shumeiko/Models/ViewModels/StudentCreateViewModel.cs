using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UniversityPortal_shumeiko.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        public string Name { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now; // Значення за замовчуванням
        // Використовуємо той самий допоміжний клас, що й для редагування
        [ValidateNever]
        public List<AssignedCourseData> AvailableCourses { get; set; }
    }
}
