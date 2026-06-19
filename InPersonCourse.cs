using SmartLearnApp.Models;

namespace week21
{
    public class InPersonCourse : Course
    {
        public string ClassroomLocation { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
    }
}