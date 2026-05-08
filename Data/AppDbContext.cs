using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student>     Students     { get; set; }
        public DbSet<Course>      Courses      { get; set; }
        public DbSet<Department>  Departments  { get; set; }
        public DbSet<Subject>     Subjects     { get; set; }
        public DbSet<Grade>       Grades       { get; set; }
        public DbSet<Enrollment>  Enrollments  { get; set; }
        public DbSet<Attendance>  Attendances  { get; set; }
        public DbSet<FeePayment>  FeePayments  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Unique admission number
            builder.Entity<Student>()
                .HasIndex(s => s.AdmissionNumber)
                .IsUnique();

            // Unique course code
            builder.Entity<Course>()
                .HasIndex(c => c.Code)
                .IsUnique();

            // Student → Course (many-to-one)
            builder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment composite uniqueness
            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            // Grade: one grade per student per subject per semester/year
            builder.Entity<Grade>()
                .HasIndex(g => new { g.StudentId, g.SubjectId, g.Semester, g.Year })
                .IsUnique();

            // Attendance: one record per student per subject per date
            builder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentId, a.SubjectId, a.Date })
                .IsUnique();

            // Decimal precision
            builder.Entity<Grade>()
                .Property(g => g.Score)
                .HasColumnType("decimal(5,2)");

            builder.Entity<Grade>()
                .Property(g => g.Points)
                .HasColumnType("decimal(3,2)");

            builder.Entity<FeePayment>()
                .Property(f => f.Amount)
                .HasColumnType("decimal(12,2)");

            builder.Entity<FeePayment>()
                .Property(f => f.Balance)
                .HasColumnType("decimal(12,2)");
        }
    }
}