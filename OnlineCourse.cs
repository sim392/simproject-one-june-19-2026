using SmartLearnApp.Models;

namespace week21
{
    public class OnlineCourse : Course
    {
        public string Platform { get; set; } = string.Empty;
        public string MeetingLink { get; set; } = string.Empty;
    }
}