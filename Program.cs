using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartLearnApp.Data;
using SmartLearnApp.Services;

Console.WriteLine("--- Application Starting ---");

var services = new ServiceCollection();

// 1. Define Connection String 
// NOTE: If localhost,1433 fails, try: "Server=.\\SQLEXPRESS;Database=smart;..."
const string connectionString = "Server=localhost,1433;Database=smart;User Id=sa;Password=SIM2003@0606;TrustServerCertificate=True;";

// 2. Setup DbContext with Resiliency
services.AddDbContext<SmartLearnDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5), // Shortened for faster debugging
            errorNumbersToAdd: null);
    }));

// 3. Register Services
services.AddScoped<StudentService>();
services.AddScoped<InstructorService>();
services.AddScoped<CourseService>();
services.AddScoped<EnrollmentService>();
services.AddScoped<AnalyticsService>();

var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

try
{
    Console.WriteLine("Step 1: Attempting to connect to Database and fetch Statistics...");
    var analyticsService = scope.ServiceProvider.GetRequiredService<AnalyticsService>();
    var stats = await analyticsService.GetSystemWideStatistics();

    Console.WriteLine("\n=== SYSTEM WIDE STATISTICS ===");
    Console.WriteLine(stats);

    Console.WriteLine("\nStep 2: Fetching Student Dashboard...");
    var studentService = scope.ServiceProvider.GetRequiredService<StudentService>();
    var dashboard = await studentService.GetStudentDashboard(1);

    if (dashboard != null)
    {
        Console.WriteLine($"\n=== STUDENT DASHBOARD: {dashboard.StudentName} ===");
        Console.WriteLine($"Total Courses: {dashboard.TotalCourses}");
        Console.WriteLine($"Completed Courses: {dashboard.CompletedCourses}");
        Console.WriteLine($"Average Progress: {dashboard.AverageProgress}%");

        foreach (var course in dashboard.Courses)
        {
            Console.WriteLine($"- {course.CourseTitle} | {course.InstructorName} | Progress: {course.Progress}% | Status: {course.Status}");
        }
    }
    else
    {
        Console.WriteLine("No dashboard found for Student ID 1.");
    }
}

catch (Exception ex)
{
    Console.WriteLine($"\n[GENERAL ERROR]: {ex.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"Inner Detail: {ex.InnerException.Message}");
}

Console.WriteLine("\n--- Process Finished. Press any key to exit ---");
Console.ReadKey();