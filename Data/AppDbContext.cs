// Data/AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Unique admission number
        builder.Entity<Student>()
            .HasIndex(s => s.AdmissionNumber)
            .IsUnique();

        // Composite key for enrollments
        builder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });
    }
}