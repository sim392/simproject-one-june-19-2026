using System.Collections.Generic;

namespace SmartLearnApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int InstructorId { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentEnrollmentCount { get; set; }

        public Instructor? Instructor { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<CourseModule> Modules { get; set; } = new List<CourseModule>();
        public ICollection<CourseRating> Ratings { get; set; } = new List<CourseRating>();
    }
}