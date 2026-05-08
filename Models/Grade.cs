// Models/Grade.cs
public class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    public decimal Score { get; set; }
    public string LetterGrade { get; set; }
    public string Semester { get; set; }
    public int Year { get; set; }
}