using SmartLearnApp.Models;

namespace week21
{
    public class HybridCourse : Course
    {
        public string OnlinePlatform { get; set; } = string.Empty;
        public string ClassroomLocation { get; set; } = string.Empty;
    }
}