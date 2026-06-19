using System;
using System.Collections.Generic;

namespace SmartLearnApp.DTOs
{
    public class StudentCourseDto
    {
        public string CourseTitle { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public double Progress { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletionDate { get; set; }
    }

    public class StudentDashboardDto
    {
        public string StudentName { get; set; } = string.Empty;
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
        public double AverageProgress { get; set; }
        public List<StudentCourseDto> Courses { get; set; } = new();
    }
}
