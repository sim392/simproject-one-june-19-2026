namespace SmartLearnApp.DTOs
{
    public class SystemWideStatisticsDto
    {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int ActiveEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public double AverageProgress { get; set; }
        public double AvgEnrollmentsPerStudent { get; set; }
        public double AvgStudentsPerCourse { get; set; }
    }

    public class TopPerformerDto
    {
        public string StudentName { get; set; } = "";
        public double AverageProgress { get; set; }
        public int CompletedCourses { get; set; }
        public int TotalEnrollments { get; set; }
    }

    public class CategoryPopularityDto
    {
        public string Category { get; set; } = "";
        public int CourseCount { get; set; }
        public int TotalEnrollments { get; set; }
        public double AverageProgress { get; set; }
        public double AverageRating { get; set; }
    }

    public class EnrollmentTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int NewEnrollments { get; set; }
    }

    public class InstructorRankingDto
    {
        public string InstructorName { get; set; } = "";
        public int CoursesTaught { get; set; }
        public int TotalStudentCount { get; set; }
        public double AverageRating { get; set; }
    }

    public class StudentAtRiskDto
    {
        public string StudentName { get; set; } = "";
        public string CourseTitle { get; set; } = "";
        public decimal ProgressPercentage { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class CourseCompletionRateDto
    {
        public string CourseTitle { get; set; } = "";
        public int TotalEnrollments { get; set; }
        public int CompletedEnrollments { get; set; }
        public double CompletionRate { get; set; }
    }
}
