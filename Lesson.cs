namespace SmartLearnApp.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public int ModuleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int OrderIndex { get; set; }
        public int DurationMinutes { get; set; }

        public CourseModule? CourseModule { get; set; }
    }
}

