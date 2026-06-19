using System;
using System.Data;
using System.Data.SqlClient;

namespace SmartLearnApp.Services
{
    public class AdoNetOperations
    {
        private const string ConnectionString =
            "Server=localhost,1433;Database=SmartLearnDB;User Id=sa;Password=SIM2003@0606;TrustServerCertificate=True;";

        // 🔹 Test Connection
        public void TestConnection()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();
                Console.WriteLine("ADO.NET connection successful.");
                Console.WriteLine("Connection state: " + conn.State);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Get All Students
        public void GetAllStudents()
        {
            string query = "SELECT UserId, Username, Email, UserType FROM Users WHERE UserType = @Role";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Role", "Student");

                using SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- Students ---");

                if (!reader.HasRows)
                {
                    Console.WriteLine("No students found.");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"ID: {reader["UserId"]}, " +
                        $"Username: {reader["Username"]}, " +
                        $"Email: {reader["Email"]}, " +
                        $"Type: {reader["UserType"]}"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Get All Courses
        public void GetAllCourses()
        {
            string query = "SELECT CourseId, Title, Category, Difficulty, MaxCapacity, CurrentEnrollmentCount FROM Courses";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- Courses ---");

                if (!reader.HasRows)
                {
                    Console.WriteLine("No courses found.");
                    return;
                }

                while (reader.Read())
                {
                    Console.WriteLine(
                        $"Course ID: {reader["CourseId"]}, " +
                        $"Title: {reader["Title"]}, " +
                        $"Category: {reader["Category"]}, " +
                        $"Difficulty: {reader["Difficulty"]}, " +
                        $"Max Capacity: {reader["MaxCapacity"]}, " +
                        $"Current Count: {reader["CurrentEnrollmentCount"]}"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Find User by ID
        public void FindUserById(int userId)
        {
            string query = "SELECT UserId, Username, Email, UserType FROM Users WHERE UserId = @UserId";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                using SqlDataReader reader = cmd.ExecuteReader();

                Console.WriteLine("\n--- Find User ---");

                if (reader.Read())
                {
                    Console.WriteLine(
                        $"ID: {reader["UserId"]}, " +
                        $"Username: {reader["Username"]}, " +
                        $"Email: {reader["Email"]}, " +
                        $"Type: {reader["UserType"]}"
                    );
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Insert Student
        public void InsertStudent(string username, string email, string passwordHash)
        {
            string query = @"INSERT INTO Users (Username, Email, PasswordHash, UserType)
                             VALUES (@Username, @Email, @PasswordHash, @UserType)";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@UserType", "Student");

                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Student inserted. Rows affected: {rows}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Insert Course
        public void InsertCourse(string title, string category, string difficulty, int maxCapacity, int instructorId)
        {
            string query = @"INSERT INTO Courses
                             (Title, Category, Difficulty, MaxCapacity, CurrentEnrollmentCount, InstructorId)
                             VALUES (@Title, @Category, @Difficulty, @MaxCapacity, 0, @InstructorId)";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Category", category);
                cmd.Parameters.AddWithValue("@Difficulty", difficulty);
                cmd.Parameters.AddWithValue("@MaxCapacity", maxCapacity);
                cmd.Parameters.AddWithValue("@InstructorId", instructorId);

                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Course inserted. Rows affected: {rows}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Update Progress
        public void UpdateStudentProgress(int enrollmentId, double progress)
        {
            string query = "UPDATE Enrollments SET ProgressPercentage = @Progress WHERE EnrollmentId = @Id";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Progress", progress);
                cmd.Parameters.AddWithValue("@Id", enrollmentId);

                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Progress updated. Rows affected: {rows}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }

        // 🔹 Delete Enrollment
        public void DeleteEnrollment(int enrollmentId)
        {
            string query = "DELETE FROM Enrollments WHERE EnrollmentId = @Id";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                conn.Open();

                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", enrollmentId);

                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"Enrollment deleted. Rows affected: {rows}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}