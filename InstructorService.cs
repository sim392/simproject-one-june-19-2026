using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Data;

namespace SmartLearnApp.Services
{
    public class InstructorService
    {
        private readonly SmartLearnDbContext _context;

        public InstructorService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public async Task<object?> GetInstructorDashboard(int instructorId)
        {
            var instructor = await _context.Instructors
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Enrollments)
                        .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(i => i.InstructorId == instructorId);

            if (instructor == null) return null;

            var result = new
            {
                Instructor = instructor.FullName,
                Courses = instructor.Courses.Select(c => new
                {
                    c.Title,
                    EnrollmentCount = c.Enrollments.Count,
                    c.MaxCapacity,
                    AverageStudentProgress = c.Enrollments.Any() ? c.Enrollments.Average(e => e.ProgressPercentage) : 0,
                    TopStudents = c.Enrollments
                        .OrderByDescending(e => e.ProgressPercentage)
                        .Take(3)
                        .Select(e => new
                        {
                            StudentName = e.Student!.FullName,
                            e.ProgressPercentage,
                            e.Status
                        }).ToList()
                }).ToList()
            };

            return result;
        }

        public async Task<object?> GetCourseAnalytics(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Ratings)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return null;

            var totalEnrollments = course.Enrollments.Count;
            var completed = course.Enrollments.Count(e => e.Status == "Completed");
            var dropped = course.Enrollments.Count(e => e.Status == "Dropped");

            return new
            {
                course.Title,
                TotalEnrollments = totalEnrollments,
                AverageProgress = totalEnrollments > 0 ? course.Enrollments.Average(e => e.ProgressPercentage) : 0,
                CompletionRate = totalEnrollments > 0 ? (double)completed * 100 / totalEnrollments : 0,
                DropoutRate = totalEnrollments > 0 ? (double)dropped * 100 / totalEnrollments : 0,
                AverageRating = course.Ratings.Any() ? course.Ratings.Average(r => r.Rating) : 0,
                ProgressDistribution = new
                {
                    ZeroToTwentyFive = course.Enrollments.Count(e => e.ProgressPercentage >= 0 && e.ProgressPercentage < 25),
                    TwentyFiveToSeventyFive = course.Enrollments.Count(e => e.ProgressPercentage >= 25 && e.ProgressPercentage < 75),
                    SeventyFiveToHundred = course.Enrollments.Count(e => e.ProgressPercentage >= 75 && e.ProgressPercentage <= 100)
                }
            };
        }

        public async Task<List<object>> GetTopStudentsInCourse(int courseId, int topN)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .OrderByDescending(e => e.ProgressPercentage)
                .Take(topN)
                .Select(e => (object)new
                {
                    StudentName = e.Student!.FullName,
                    Progress = e.ProgressPercentage,
                    e.Status,
                    e.EnrollmentDate
                })
                .ToListAsync();
        }

        public async Task<List<object>> GetStudentsAtRisk()
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);

            return await _context.Enrollments
                .Where(e => e.Status == "Active"
                            && e.ProgressPercentage < 30
                            && e.EnrollmentDate < twoWeeksAgo)
                .Include(e => e.Student)
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Instructor)
                .Where(e => e.Course!.InstructorId > 0)
                .Select(e => (object)new
                {
                    StudentName = e.Student!.FullName,
                    CourseTitle = e.Course!.Title,
                    InstructorName = e.Course!.Instructor!.FullName,
                    e.ProgressPercentage,
                    e.EnrollmentDate
                })
                .ToListAsync();
        }
    }
}