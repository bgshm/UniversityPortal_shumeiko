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
        // IBlobService - это имя, которое я использовал в предыдущем ответе. 
        // Если у вас он называется IFileService, используйте ваше имя.
        private readonly IBlobService _blobService;

        public ProfessorsController(IProfessorService professorService, IBlobService blobService)
        {
            _professorService = professorService;
            _blobService = blobService; // Убедитесь, что это IBlobService (или IFileService)
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
            return View(new ProfessorCreateViewModel());
        }

        // POST: Professors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfessorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var professor = new Professor
                {
                    Name = viewModel.Name,
                    Specialization = viewModel.Specialization
                };

                if (viewModel.ProfilePictureFile != null && viewModel.ProfilePictureFile.Length > 0)
                {
                    // Предполагается, что ваш сервис возвращает URL
                    professor.ProfilePictureUrl = await _blobService.UploadFileAsync(viewModel.ProfilePictureFile);
                }

                await _professorService.CreateProfessorAsync(professor);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Professors/Edit/5 (ОБНОВЛЕНО)
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

            // Мапим модель в ViewModel
            var viewModel = new ProfessorEditViewModel
            {
                Id = professor.Id,
                Name = professor.Name,
                Specialization = professor.Specialization,
                CurrentProfilePictureUrl = professor.ProfilePictureUrl
            };

            return View(viewModel); // Передаем ViewModel в View
        }

        // POST: Professors/Edit/5 (ОБНОВЛЕНО)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProfessorEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Получаем существующую сущность из БД
                    var professorToUpdate = await _professorService.GetProfessorByIdAsync(id);
                    if (professorToUpdate == null)
                    {
                        return NotFound();
                    }

                    // 2. Проверяем, был ли загружен *новый* файл
                    if (viewModel.ProfilePictureFile != null && viewModel.ProfilePictureFile.Length > 0)
                    {
                        // 3. Если старое фото существует, удаляем его из Blob
                        if (!string.IsNullOrEmpty(professorToUpdate.ProfilePictureUrl))
                        {
                            // Извлекаем имя blob из URL для удаления
                            var oldBlobName = new Uri(professorToUpdate.ProfilePictureUrl).Segments.Last();
                            await _blobService.DeleteFileAsync(oldBlobName);
                        }

                        // 4. Загружаем новый файл и получаем новый URL
                        professorToUpdate.ProfilePictureUrl = await _blobService.UploadFileAsync(viewModel.ProfilePictureFile);
                    }

                    // 5. Обновляем остальные данные
                    professorToUpdate.Name = viewModel.Name;
                    professorToUpdate.Specialization = viewModel.Specialization;

                    await _professorService.UpdateProfessorAsync(professorToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _professorService.ProfessorExistsAsync(viewModel.Id))
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
            // Если модель невалидна, возвращаем ViewModel (CurrentProfilePictureUrl сохранится)
            return View(viewModel);
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

        // POST: Professors/Delete/5 (ОБНОВЛЕНО)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var professor = await _professorService.GetProfessorByIdAsync(id);
            if (professor == null)
            {
                return NotFound();
            }

            // 1. Удаляем фото из Blob Storage, если оно есть
            if (!string.IsNullOrEmpty(professor.ProfilePictureUrl))
            {
                try
                {
                    var blobName = new Uri(professor.ProfilePictureUrl).Segments.Last();
                    await _blobService.DeleteFileAsync(blobName);
                }
                catch (Exception ex)
                {
                    // Залогировать ошибку, но продолжить удаление из БД
                    Console.WriteLine($"Error deleting blob: {ex.Message}");
                }
            }

            // 2. Удаляем запись из БД
            await _professorService.DeleteProfessorAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
