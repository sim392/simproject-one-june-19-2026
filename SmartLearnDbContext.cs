using Microsoft.EntityFrameworkCore;
using SmartLearnApp.Models;

namespace SmartLearnApp.Data
{
    public class SmartLearnDbContext : DbContext
    {
        // ✅ Added for EF design-time support
        public SmartLearnDbContext()
        {
        }


    public SmartLearnDbContext(DbContextOptions<SmartLearnDbContext> options)
        : base(options)
        {
        }

        // ✅ Added fallback connection for migrations
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=localhost,1433;Database=smart;User Id=sa;Password=SIM2003@0606;TrustServerCertificate=True;",
                    options => options.EnableRetryOnFailure()
                );
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course → Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student → Enrollments
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course → Modules
            modelBuilder.Entity<CourseModule>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Module → Lessons
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.CourseModule)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course → Ratings
            modelBuilder.Entity<CourseRating>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Ratings)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student → Ratings
            modelBuilder.Entity<CourseRating>()
                .HasOne(r => r.Student)
                .WithMany(s => s.Ratings)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: one rating per student per course
            modelBuilder.Entity<CourseRating>()
                .HasIndex(r => new { r.StudentId, r.CourseId })
                .IsUnique();

            // Rating check constraint (1–5)
            modelBuilder.Entity<CourseRating>()
                .HasCheckConstraint(
                    "CK_CourseRating_Rating",
                    "[Rating] >= 1 AND [Rating] <= 5"
                );
        }
    }


}
