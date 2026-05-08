// Models/Student.cs
public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string AdmissionNumber { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Grade> Grades { get; set; }
}