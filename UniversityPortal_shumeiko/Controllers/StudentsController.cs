using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityPortal_shumeiko.Models;
using UniversityPortal_shumeiko.Models.ViewModels;
using UniversityPortal_shumeiko.Services;

namespace UniversityPortal_shumeiko.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public StudentsController(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            var allCourses = await _courseService.GetAllCoursesAsync();
            var viewModel = new StudentCreateViewModel
            {
                AvailableCourses = allCourses.Select(c => new AssignedCourseData
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    Assigned = false
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: Students/Create
        [HttpPost]

        // ВИПРАВЛЕНО: int selectedCourses -> int selectedCourses
        public async Task<IActionResult> Create(StudentCreateViewModel viewModel, int[] selectedCourses)
        {
            if (ModelState.IsValid)
            {
                var newStudent = new Student
                {
                    Name = viewModel.Name,
                    EnrollmentDate = viewModel.EnrollmentDate,
                    Enrollments = new List<Enrollment>()
                };

                if (selectedCourses != null)
                {
                    foreach (var courseId in selectedCourses)
                    {
                        newStudent.Enrollments.Add(new Enrollment { CourseId = courseId });
                    }
                }

                await _studentService.CreateStudentAsync(newStudent);
                return RedirectToAction(nameof(Index));
            }

            await RepopulateCoursesForViewModel(viewModel, selectedCourses);
            return View(viewModel);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Используем AsNoTracking() для GET, так как мы не будем обновлять этот объект
            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null) return NotFound();

            var viewModel = new StudentEditViewModel
            {
                Student = student,
                AssignedCourses = await PopulateAssignedCourseData(student)
            };
            return View(viewModel);
        }
        // niger

        // POST: Students/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, int[] selectedCourses)
        {
            var studentToUpdate = await _studentService.GetStudentByIdForUpdateAsync(id);
            if (studentToUpdate == null)
            {
                return NotFound();
            }

            // isModelValid будет false, если Name пустое или EnrollmentDate - не дата

            if (ModelState.IsValid) // Сценарий 1: УСПЕХ
            {
                // Валидация прошла, обновляем курсы
                UpdateStudentCourses(selectedCourses, studentToUpdate);
                await _studentService.UpdateStudentAsync(studentToUpdate);
                return RedirectToAction(nameof(Index));
            }

            // Сценарий 2: ОШИБКА ВАЛИДАЦИИ (isModelValid == false)
            // Код доходит сюда. "Галочка перепрыгивает", потому что
            // вы не перестраиваете ViewModel с учетом `selectedCourses`.

            // --- ВОТ ИСПРАВЛЕНИЕ ---

            // 1. Получаем ВСЕ курсы, доступные для выбора
            var allCourses = await _courseService.GetAllCoursesAsync();

            // 2. Создаем HashSet из тех ID, что пришли из формы (selectedCourses)
            var selectedCoursesHS = new HashSet<int>(selectedCourses ?? Array.Empty<int>());

            // 3. Строим список чекбоксов ЗАНОВО
            var assignedCoursesViewModel = allCourses.Select(course => new AssignedCourseData
            {
                CourseId = course.Id,
                Title = course.Title,
                // Ставим 'Assigned = true' для тех, кто был в selectedCourses
                Assigned = selectedCoursesHS.Contains(course.Id)
            }).ToList();

            // 4. Возвращаем ViewModel с НОВЫМ списком чекбоксов
            var viewModel = new StudentEditViewModel
            {
                // studentToUpdate уже содержит невалидные данные (например, пустое Name)
                Student = studentToUpdate,
                // Передаем список, который мы ТОЛЬКО ЧТО создали
                AssignedCourses = assignedCoursesViewModel
            };

            // Теперь пользователь увидит и ошибку валидации, и свои галочки
            return View(viewModel);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Students/Delete/5

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _studentService.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // --- Допоміжні методи ---

        private async Task<List<AssignedCourseData>> PopulateAssignedCourseData(Student student)
        {
            var allCourses = await _courseService.GetAllCoursesAsync();
            var studentCourseIds = new HashSet<int>(student.Enrollments.Select(c => c.CourseId));
            var viewModel = new List<AssignedCourseData>();

            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseId = course.Id,
                    Title = course.Title,
                    Assigned = studentCourseIds.Contains(course.Id)
                });
            }
            return viewModel;
        }

        private void UpdateStudentCourses(int[] selectedCourses, Student studentToUpdate)
        {
            var selectedCoursesHS = new HashSet<int>(selectedCourses ?? Array.Empty<int>());
            var studentCoursesHS = new HashSet<int>(studentToUpdate.Enrollments.Select(e => e.CourseId));

            foreach (var courseId in selectedCoursesHS)
            {
                // Добавляем новые
                if (!studentCoursesHS.Contains(courseId))
                {
                    studentToUpdate.Enrollments.Add(new Enrollment { CourseId = courseId });
                }
            }

            foreach (var enrollment in studentToUpdate.Enrollments.ToList())
            {
                // Удаляем старые
                if (!selectedCoursesHS.Contains(enrollment.CourseId))
                {
                    studentToUpdate.Enrollments.Remove(enrollment);
                }
            }
        }

        private async Task RepopulateCoursesForViewModel(StudentCreateViewModel viewModel, int[] selectedCourses)
        {
            var allCourses = await _courseService.GetAllCoursesAsync();
            var selectedCoursesHS = new HashSet<int>(selectedCourses ?? Array.Empty<int>());
            viewModel.AvailableCourses = allCourses.Select(c => new AssignedCourseData
            {
                CourseId = c.Id,
                Title = c.Title,
                Assigned = selectedCoursesHS.Contains(c.Id)
            }).ToList();
        }
    }
}
