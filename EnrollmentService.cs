using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Data;
using SmartLearnApp.Models;

namespace SmartLearnApp.Services
{
    public class EnrollmentService
    {
        private readonly SmartLearnDbContext _context;

        public EnrollmentService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public async Task<string> EnrollStudent(int studentId, int courseId)
        {
            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Status != "Dropped");

            if (existing != null)
                return "Student is already enrolled in this course.";

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return "Course not found.";

            if (course.CurrentEnrollmentCount >= course.MaxCapacity)
                return "Course is full.";

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Active",
                ProgressPercentage = 0
            };

            _context.Enrollments.Add(enrollment);
            course.CurrentEnrollmentCount++;

            await _context.SaveChangesAsync();
            return "Student enrolled successfully.";
        }

        public async Task<string> DropCourse(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return "Enrollment not found.";

            enrollment.Status = "Dropped";

            var course = await _context.Courses.FindAsync(courseId);
            if (course != null && course.CurrentEnrollmentCount > 0)
                course.CurrentEnrollmentCount--;

            await _context.SaveChangesAsync();
            return "Course dropped successfully.";
        }

        public async Task<string> MarkAsCompleted(int enrollmentId)
        {
            var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
            if (enrollment == null)
                return "Enrollment not found.";

            enrollment.Status = "Completed";
            enrollment.ProgressPercentage = 100;
            enrollment.CompletionDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return "Enrollment marked as completed.";
        }

        public async Task<List<object>> GetEnrollmentTrends()
        {
            return await _context.Enrollments
                .GroupBy(e => new { e.EnrollmentDate.Year, e.EnrollmentDate.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => (object)new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    EnrollmentCount = g.Count()
                })
                .ToListAsync();
        }
    }
}