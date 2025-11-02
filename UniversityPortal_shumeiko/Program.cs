using Microsoft.EntityFrameworkCore;
using UniversityPortal_shumeiko.Data;
using UniversityPortal_shumeiko.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProfessorService, ProfessorService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IBlobService, BlobService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ---== АВТОМАТИЧЕСКОЕ ПРИМЕНЕНИЕ МИГРАЦИЙ EF CORE ==---
//: Создаем новый scope для получения scoped-сервиса (DbContext)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Получаем наш DbContext
        var context = services.GetRequiredService<ApplicationDbContext>();

        //: Выполняем миграции
        context.Database.Migrate();

        // Опционально: здесь можно добавить инициализатор данных (Seed)
        // DbInitializer.Initialize(context); 
    }
    catch (Exception ex)
    {
        //: Логгируем ошибку, если миграция не удалась
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        // Это часто происходит, если строка подключения в docker-compose неверна
    }
}
// ---== КОНЕЦ СЕКЦИИ МИГРАЦИЙ ==---

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
