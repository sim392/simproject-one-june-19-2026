using System;

namespace SmartLearnApp.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Completed, Dropped
        public double ProgressPercentage { get; set; }
        public DateTime? CompletionDate { get; set; }

        public User? Student { get; set; }
        public Course? Course { get; set; }
    }
}