using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniversityPortal_shumeiko.Data;
using UniversityPortal_shumeiko.Models;
using UniversityPortal_shumeiko.Models.ViewModels;
using UniversityPortal_shumeiko.Services;

namespace UniversityPortal_shumeiko.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IProfessorService _professorService; // Потрібен для отримання списку професорів

        public CoursesController(ICourseService courseService, IProfessorService professorService)
        {
            _courseService = courseService;
            _professorService = professorService;
        }

        // GET: Courses
        // Відображає список усіх курсів
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        // Відображає детальну інформацію про один курс
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProfessorId"] = new SelectList(await _professorService.GetAllProfessorsAsync(), "Id", "Name");
            // Передаємо у подання порожню ViewModel
            return View(new CourseCreateViewModel());
        }

        // POST: Courses/Create
        [HttpPost]

        public async Task<IActionResult> Create(CourseCreateViewModel viewModel) // <-- Приймаємо ViewModel
        {
            // Перевіряємо валідність ViewModel, а не Course
            if (ModelState.IsValid)
            {
                // Створюємо доменну модель Course вручну з даних ViewModel
                var course = new Course
                {
                    Title = viewModel.Title,
                    Credits = viewModel.Credits,
                    Department = viewModel.Department,
                    ProfessorId = viewModel.ProfessorId
                };

                await _courseService.CreateCourseAsync(course);
                return RedirectToAction(nameof(Index));
            }

            // Якщо модель не валідна, повертаємо ту саму ViewModel на форму
            ViewData["ProfessorId"] = new SelectList(await _professorService.GetAllProfessorsAsync(), "Id", "Name", viewModel.ProfessorId);
            return View(viewModel);
        }

        // GET: Courses/Edit/5
        // Отображает форму для редактирования
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            // 1. Преобразуем модель Course в CourseEditViewModel
            var viewModel = new CourseEditViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Credits = course.Credits,
                Department = course.Department,
                ProfessorId = course.ProfessorId
            };

            // 2. Передаем список профессоров для выпадающего списка
            ViewData["ProfessorId"] = new SelectList(await _professorService.GetAllProfessorsAsync(), "Id", "Name", course.ProfessorId);

            // 3. Передаем ViewModel в представление
            return View(viewModel);
        }

        // POST: Courses/Edit/5
        // Принимает данные из формы
        [HttpPost]

        public async Task<IActionResult> Edit(int id, CourseEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Теперь ModelState.IsValid будет работать, так как он проверяет ViewModel,
            // в которой нет "Professor", а есть только "ProfessorId"
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Получаем оригинальную сущность из БД
                    var courseToUpdate = await _courseService.GetCourseByIdForUpdateAsync(viewModel.Id);
                    if (courseToUpdate == null)
                    {
                        return NotFound();
                    }

                    // 2. Вручную обновляем ее свойства из ViewModel
                    courseToUpdate.Title = viewModel.Title;
                    courseToUpdate.Credits = viewModel.Credits;
                    courseToUpdate.Department = viewModel.Department;
                    courseToUpdate.ProfessorId = viewModel.ProfessorId;

                    // 3. Сохраняем обновленную сущность
                    await _courseService.UpdateCourseAsync(courseToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _courseService.CourseExistsAsync(viewModel.Id))
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

            // Если модель не валидна, возвращаем пользователя на форму
            ViewData["ProfessorId"] = new SelectList(await _professorService.GetAllProfessorsAsync(), "Id", "Name", viewModel.ProfessorId);
            return View(viewModel);
        }

        // GET: Courses/Delete/5
        // Відображає сторінку підтвердження видалення курсу
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseService.GetCourseByIdAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        // Видаляє курс з бази даних після підтвердження

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
