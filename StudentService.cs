using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Data;
using SmartLearnApp.DTOs;
using SmartLearnApp.Models;

namespace SmartLearnApp.Services
{
    public class StudentService
    {
        private readonly SmartLearnDbContext _context;

        public StudentService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public async Task<StudentDashboardDto?> GetStudentDashboard(int studentId)
        {
            var student = await _context.Users
    .Where(s => s.UserId == studentId && s.Role == "Student")
    .Select(s => new StudentDashboardDto
    {
        StudentName = s.Username,

        TotalCourses = s.Enrollments.Count(),

        CompletedCourses = s.Enrollments.Count(e => e.Status == "Completed"),

        AverageProgress = s.Enrollments.Any()
            ? s.Enrollments.Average(e => e.ProgressPercentage)
            : 0,

        Courses = s.Enrollments.Select(e => new StudentCourseDto
        {
            CourseTitle = e.Course != null ? e.Course.Title : "",
            InstructorName = e.Course != null && e.Course.Instructor != null
                ? e.Course.Instructor.FullName
                : "",
            Progress = e.ProgressPercentage,
            Status = e.Status,
            CompletionDate = e.CompletionDate
        }).ToList()
    })
    .FirstOrDefaultAsync();

            return student;
        }

        public async Task<List<StudentCourseDto>> GetStudentCompletedCourses(int studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == "Completed")
                .Include(e => e.Course)
                .Select(e => new StudentCourseDto
                {
                    CourseTitle = e.Course!.Title,
                    InstructorName = e.Course.Instructor != null ? e.Course.Instructor.FullName : "",
                    Progress = e.ProgressPercentage,
                    Status = e.Status,
                    CompletionDate = e.CompletionDate
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateCourseProgress(int studentId, int courseId, double newProgress)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null) return false;

            enrollment.ProgressPercentage = newProgress;

            if (newProgress >= 100)
            {
                enrollment.ProgressPercentage = 100;
                enrollment.Status = "Completed";
                enrollment.CompletionDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<StudentCourseDto>> GetStudentAtRiskCourses(int studentId)
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);

            return await _context.Enrollments
                .Where(e => e.StudentId == studentId
                            && e.Status == "Active"
                            && e.ProgressPercentage < 30
                            && e.EnrollmentDate < twoWeeksAgo)
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Instructor)
                .Select(e => new StudentCourseDto
                {
                    CourseTitle = e.Course!.Title,
                    InstructorName = e.Course!.Instructor != null ? e.Course.Instructor.FullName : "",
                    Progress = e.ProgressPercentage,
                    Status = e.Status,
                    CompletionDate = e.CompletionDate
                })
                .ToListAsync();
        }
    }
}