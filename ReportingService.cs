using System;
using System.Collections.Generic;
using System.Linq;
using SmartLearnApp.Data;
using SmartLearnApp.Models;

namespace week21
{
    public class ReportingService
    {
        private readonly SmartLearnDbContext _context;

        public ReportingService(SmartLearnDbContext context)
        {
            _context = context;
        }

        public int GetTotalStudents()
        {
            try
            {
                return _context.Students.Count(u => u.Role == "Student");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTotalStudents error: {ex.Message}");
                return 0;
            }
        }

        public double GetAverageProgress()
        {
            try
            {
                if (!_context.Enrollments.Any())
                    return 0;

                return _context.Enrollments.Average(e => e.ProgressPercentage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAverageProgress error: {ex.Message}");
                return 0;
            }
        }

        public Course? GetCourseWithMaxEnrollments()
        {
            try
            {
                return _context.Courses
                    .OrderByDescending(c => c.CurrentEnrollmentCount)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCourseWithMaxEnrollments error: {ex.Message}");
                return null;
            }
        }

        public List<Enrollment> GetAllEnrollments()
        {
            try
            {
                return _context.Enrollments.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllEnrollments error: {ex.Message}");
                return new List<Enrollment>();
            }
        }
    }
}