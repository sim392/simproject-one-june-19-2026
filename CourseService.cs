using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Data;

namespace SmartLearnApp.Services
{
    public class CourseService
    {
        private readonly SmartLearnDbContext _context;

        public CourseService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public async Task<List<object>> SearchCourses(string? keyword, string? category, string? difficulty)
        {
            var query = _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c =>
                    c.Title.Contains(keyword) ||
                    (c.Description != null && c.Description.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(c => c.Category == category);

            if (!string.IsNullOrWhiteSpace(difficulty))
                query = query.Where(c => c.Difficulty == difficulty);

            return await query
                .Select(c => (object)new
                {
                    c.CourseId,
                    c.Title,
                    c.Category,
                    c.Difficulty,
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                    EnrollmentCount = c.Enrollments.Count
                })
                .ToListAsync();
        }

        public async Task<List<object>> GetPopularCourses(int topN)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Ratings)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(topN)
                .Select(c => (object)new
                {
                    c.Title,
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                    c.Category,
                    EnrollmentCount = c.Enrollments.Count,
                    AverageRating = c.Ratings.Any() ? c.Ratings.Average(r => r.Rating) : 0
                })
                .ToListAsync();
        }

        public async Task<object?> GetCourseWithFullDetails(int courseId)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .Include(c => c.Ratings)
                    .ThenInclude(r => r.Student)
                .Where(c => c.CourseId == courseId)
                .Select(c => new
                {
                    c.CourseId,
                    c.Title,
                    c.Description,
                    c.Category,
                    c.Difficulty,
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                    Modules = c.Modules.OrderBy(m => m.OrderIndex).Select(m => new
                    {
                        m.ModuleId,
                        m.Title,
                        m.Description,
                        m.OrderIndex,
                        m.DurationHours,
                        Lessons = m.Lessons.OrderBy(l => l.OrderIndex).Select(l => new
                        {
                            l.LessonId,
                            l.Title,
                            l.Content,
                            l.VideoUrl,
                            l.OrderIndex,
                            l.DurationMinutes
                        }).ToList()
                    }).ToList(),
                    Enrollments = c.Enrollments.Select(e => new
                    {
                        StudentName = e.Student != null ? e.Student.FullName : "",
                        e.Status,
                        e.ProgressPercentage,
                        e.EnrollmentDate
                    }).ToList(),
                    Ratings = c.Ratings.Select(r => new
                    {
                        StudentName = r.Student != null ? r.Student.FullName : "",
                        r.Rating,
                        r.ReviewText,
                        r.RatingDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<object>> GetRecommendedCourses(int studentId)
        {
            var studentCategories = await _context.Enrollments
                .Where(e => e.StudentId == studentId &&
                            (e.Status == "Completed" || e.Status == "Active"))
                .Include(e => e.Course)
                .Select(e => e.Course!.Category)
                .Distinct()
                .ToListAsync();

            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Ratings)
                .Where(c => !enrolledCourseIds.Contains(c.CourseId)
                            && studentCategories.Contains(c.Category))
                .OrderByDescending(c => c.Ratings.Any() ? c.Ratings.Average(r => r.Rating) : 0)
                .Select(c => (object)new
                {
                    c.CourseId,
                    c.Title,
                    c.Category,
                    c.Difficulty,
                    InstructorName = c.Instructor != null ? c.Instructor.FullName : "",
                    AverageRating = c.Ratings.Any() ? c.Ratings.Average(r => r.Rating) : 0
                })
                .ToListAsync();
        }
    }
}