// Models/Course.cs
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public int Credits { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public ICollection<Student> Students { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
}
