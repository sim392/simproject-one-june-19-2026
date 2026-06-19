using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Data;
using SmartLearnApp.Models;

namespace week21
{
    public class AdvancedQueryService
    {
        private readonly SmartLearnDbContext _context;

        public AdvancedQueryService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public List<Course> GetTopCoursesByEnrollment()
        {
            try
            {
                return _context.Courses
                    .OrderByDescending(c => c.CurrentEnrollmentCount)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTopCoursesByEnrollment error: {ex.Message}");
                return new List<Course>();
            }
        }

        public List<Student> GetStudentsWithProgressGreaterThan80()
        {
            try
            {
                var students = _context.Students
                    .Where(u => u.Role == "Student" &&
                                u.Enrollments.Any(e => e.ProgressPercentage > 80))
                    .ToList();

                return students.Select(u => new Student
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash,
                    Role = u.Role,
                    LastLoginDate = u.LastLoginDate
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetStudentsWithProgressGreaterThan80 error: {ex.Message}");
                return new List<Student>();
            }
        }

        public List<Course> GetCoursesByCategoryWithInstructor(string category)
        {
            try
            {
                return _context.Courses
                    .Include(c => c.Instructor)
                    .Where(c => c.Category == category)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCoursesByCategoryWithInstructor error: {ex.Message}");
                return new List<Course>();
            }
        }

        public List<Enrollment> GetActiveEnrollments()
        {
            try
            {
                return _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Status == "Active")
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetActiveEnrollments error: {ex.Message}");
                return new List<Enrollment>();
            }
        }
    }
}