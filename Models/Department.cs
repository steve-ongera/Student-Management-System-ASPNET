using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Code { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Head of Department")]
        public string? HeadOfDepartment { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}