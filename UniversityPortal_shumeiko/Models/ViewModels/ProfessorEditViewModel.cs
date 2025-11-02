using System.ComponentModel.DataAnnotations;

namespace UniversityPortal_shumeiko.Models.ViewModels
{
    public class ProfessorEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Specialization { get; set; }

        // Это для загрузки *нового* файла
        [Display(Name = "Нове фото профілю")]
        public IFormFile? ProfilePictureFile { get; set; }

        // Это для отображения *текущего* фото
        public string? CurrentProfilePictureUrl { get; set; }
    }
}
