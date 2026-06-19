using System.ComponentModel.DataAnnotations;

namespace SmartLearnApp.Models
{
    public class CourseRating
    {
        [Key]
        public int RatingId { get; set; }
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime RatingDate { get; set; }

        public Course? Course { get; set; }
        public User? Student { get; set; }
    }
}