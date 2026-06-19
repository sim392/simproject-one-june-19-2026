using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartLearnApp.Models
{
    public class CourseModule
    {
        [Key]   // ✅ ADD THIS
        public int ModuleId { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int OrderIndex { get; set; }

        public double DurationHours { get; set; }

        public Course? Course { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
