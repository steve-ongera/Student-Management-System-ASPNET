using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        public int Credits { get; set; } = 3;

        public int YearOfStudy { get; set; } = 1;

        [MaxLength(20)]
        public string Semester { get; set; } = "Semester 1";

        // FK
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        // Navigation
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}