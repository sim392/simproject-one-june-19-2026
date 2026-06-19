using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Data;
using SmartLearnApp.DTOs;

namespace SmartLearnApp.Services
{
    public class AnalyticsService
    {
        private readonly SmartLearnDbContext _context;

        public AnalyticsService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public async Task<SystemWideStatisticsDto> GetSystemWideStatistics()
        {
            var totalStudents = await _context.Users
    .CountAsync(u => u.Role == "Student");

            var totalInstructors = await _context.Users
                .CountAsync(u => u.Role == "Instructor");
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();

            var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Active");
            var completedEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Completed");

           

            var avgEnrollmentsPerStudent = totalStudents > 0 ? (double)totalEnrollments / totalStudents : 0;
            var avgStudentsPerCourse = totalCourses > 0 ? (double)totalEnrollments / totalCourses : 0;

            return new SystemWideStatisticsDto
            {
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalCourses = totalCourses,
                TotalEnrollments = totalEnrollments,
                ActiveEnrollments = activeEnrollments,
                CompletedEnrollments = completedEnrollments,
                
                AvgEnrollmentsPerStudent = avgEnrollmentsPerStudent,
                AvgStudentsPerCourse = avgStudentsPerCourse
            };
        }

        public async Task<List<TopPerformerDto>> GetTopPerformers(int topN)
        {
            return await _context.Users
    .Where(u => u.Role == "Student")
    .Include(u => u.Enrollments)
    .Select(u => new TopPerformerDto
    {
        StudentName = u.FullName,
       
        CompletedCourses = u.Enrollments.Count(e => e.Status == "Completed"),
        TotalEnrollments = u.Enrollments.Count
    })
    .OrderByDescending(s => s.AverageProgress)
    .Take(topN)
    .ToListAsync();
        }

        public async Task<List<CategoryPopularityDto>> GetCategoryPopularity()
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Ratings)
                .GroupBy(c => c.Category)
                .Select(g => new CategoryPopularityDto
                {
                    Category = g.Key,
                    CourseCount = g.Count(),
                    TotalEnrollments = g.Sum(c => c.Enrollments.Count),
                    
                    AverageRating = g.SelectMany(c => c.Ratings).Any()
                        ? g.SelectMany(c => c.Ratings).Average(r => r.Rating)
                        : 0
                })
                .OrderByDescending(x => x.TotalEnrollments)
                .ToListAsync();
        }

        public async Task<List<EnrollmentTrendDto>> GetEnrollmentTrendsByMonth()
        {
            return await _context.Enrollments
                .GroupBy(e => new { e.EnrollmentDate.Year, e.EnrollmentDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new EnrollmentTrendDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    NewEnrollments = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<InstructorRankingDto>> GetInstructorRankings()
        {
           return await _context.Users
    .Where(u => u.Role == "Instructor")
    .Include(u => u.CoursesTaught)
        .ThenInclude(c => c.Enrollments)
    .Include(u => u.CoursesTaught)
        .ThenInclude(c => c.Ratings)
    .Select(u => new InstructorRankingDto
    {
        InstructorName = u.FullName,
        CoursesTaught = u.CoursesTaught.Count,
        TotalStudentCount = u.CoursesTaught
            .SelectMany(c => c.Enrollments)
            .Select(e => e.StudentId)
            .Distinct()
            .Count(),
        AverageRating = u.CoursesTaught
            .SelectMany(c => c.Ratings)
            .Any()
            ? u.CoursesTaught.SelectMany(c => c.Ratings).Average(r => r.Rating)
            : 0
    })
    .OrderByDescending(x => x.TotalStudentCount)
    .ToListAsync();
        }

        public async Task<List<StudentAtRiskDto>> GetStudentsAtRisk()
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);

            return await _context.Enrollments
                .Where(e => e.Status == "Active"
                            && e.ProgressPercentage < 30
                            && e.EnrollmentDate < twoWeeksAgo)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new StudentAtRiskDto
                {
                    StudentName = e.Student != null ? e.Student.FullName : "",
                    CourseTitle = e.Course != null ? e.Course.Title : "",
                   
                   EnrollmentDate = e.EnrollmentDate
                })
                .ToListAsync();
        }

        public async Task<List<CourseCompletionRateDto>> GetCourseCompletionRates()
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .Select(c => new CourseCompletionRateDto
                {
                    CourseTitle = c.Title,
                    TotalEnrollments = c.Enrollments.Count,
                    CompletedEnrollments = c.Enrollments.Count(e => e.Status == "Completed"),
                    CompletionRate = c.Enrollments.Count > 0
                        ? (double)c.Enrollments.Count(e => e.Status == "Completed") * 100 / c.Enrollments.Count
                        : 0
                })
                .OrderByDescending(x => x.CompletionRate)
                .ToListAsync();
        }
    }
}