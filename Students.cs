using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLearnApp.Models
{
    public class Student : User
    {
        public Student()
        {
            Role = "Student";
        }

        // ✅ This maps StudentId to UserId (VERY IMPORTANT)
        [NotMapped]
        public int StudentId => UserId;

        public List<CourseRating> CourseRatings { get; set; } = new();
    }
}