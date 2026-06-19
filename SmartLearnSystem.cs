using System;
using System.Collections.Generic;
using SmartLearnApp.Models;

namespace week21
{
    public class SmartLearnSystem
    {
        public List<User> Users { get; set; }
        public List<Course> Courses { get; set; }
        public List<Enrollment> Enrollments { get; set; }

        public SmartLearnSystem()
        {
            Users = new List<User>();
            Courses = new List<Course>();
            Enrollments = new List<Enrollment>();
        }

        public void AddUser(User user)
        {
            if (user != null)
                Users.Add(user);
        }

        public void AddCourse(Course course)
        {
            if (course != null)
                Courses.Add(course);
        }

        public void AddEnrollment(Enrollment enrollment)
        {
            if (enrollment != null)
                Enrollments.Add(enrollment);
        }
    }
}