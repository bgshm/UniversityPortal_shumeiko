using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityPortal_shumeiko.Models;
using UniversityPortal_shumeiko.Models.ViewModels;
using UniversityPortal_shumeiko.Services;

namespace UniversityPortal_shumeiko.Controllers
{
    public class ProfessorsController : Controller
    {
        private readonly IProfessorService _professorService;
        private readonly IFileService _fileService;

        public ProfessorsController(IProfessorService professorService, IFileService fileService)
        {
            _professorService = professorService;
            _fileService = fileService;
        }

        // GET: Professors
        public async Task<IActionResult> Index()
        {
            var professors = await _professorService.GetAllProfessorsAsync();
            return View(professors);
        }

        // GET: Professors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _professorService.GetProfessorByIdAsync(id.Value);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // GET: Professors/Create
        public IActionResult Create()
        {
            // Передаємо порожню ViewModel у подання
            return View(new ProfessorCreateViewModel());
        }

        // POST: Professors/Create
        [HttpPost]

        public async Task<IActionResult> Create(ProfessorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Ручне відображення (мапінг) даних з ViewModel на доменну модель
                var professor = new Professor
                {
                    Name = viewModel.Name,
                    Specialization = viewModel.Specialization
                };

                if (viewModel.ProfilePictureFile != null && viewModel.ProfilePictureFile.Length > 0)
                {
                    professor.ProfilePictureUrl = await _fileService.SaveFileAsync(viewModel.ProfilePictureFile, "professors");
                }

                await _professorService.CreateProfessorAsync(professor);
                return RedirectToAction(nameof(Index));
            }
            // Якщо валідація не пройдена, повертаємо ту саму ViewModel у подання
            return View(viewModel);
        }

        // GET: Professors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _professorService.GetProfessorByIdAsync(id.Value);
            if (professor == null)
            {
                return NotFound();
            }
            return View(professor);
        }

        // POST: Professors/Edit/5
        [HttpPost]

        public async Task<IActionResult> Edit(int id, Professor professor, IFormFile? profilePictureFile)
        {
            if (id != professor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Отримуємо поточні дані професора з бази, щоб мати доступ до старого шляху файлу
                    var professorToUpdate = await _professorService.GetProfessorByIdAsync(id);
                    if (professorToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Перевіряємо, чи було завантажено новий файл
                    if (profilePictureFile != null && profilePictureFile.Length > 0)
                    {
                        // Якщо існує старе фото, видаляємо його
                        if (!string.IsNullOrEmpty(professorToUpdate.ProfilePictureUrl))
                        {
                            _fileService.DeleteFile(professorToUpdate.ProfilePictureUrl);
                        }

                        // Зберігаємо новий файл і оновлюємо шлях
                        professorToUpdate.ProfilePictureUrl = await _fileService.SaveFileAsync(profilePictureFile, "professors");
                    }

                    // Оновлюємо інші властивості моделі
                    professorToUpdate.Name = professor.Name;
                    professorToUpdate.Specialization = professor.Specialization;

                    // Зберігаємо оновлену сутність в базі даних
                    await _professorService.UpdateProfessorAsync(professorToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _professorService.ProfessorExistsAsync(professor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(professor);
        }

        // GET: Professors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professor = await _professorService.GetProfessorByIdAsync(id.Value);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // POST: Professors/Delete/5

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _professorService.DeleteProfessorAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
