using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db          = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await db.Database.MigrateAsync();

            // ── Roles ──────────────────────────────────────────────────────────
            string[] roles = { "Admin", "Teacher", "Student" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ── Default Users ──────────────────────────────────────────────────
            await EnsureUser(userManager, "admin@school.com",   "Admin@1234",   "Admin");
            await EnsureUser(userManager, "teacher@school.com", "Teacher@1234", "Teacher");
            await EnsureUser(userManager, "student@school.com", "Student@1234", "Student");

            if (db.Departments.Any()) return;   // already seeded

            // ── Departments ────────────────────────────────────────────────────
            var depts = new List<Department>
            {
                new() { Name = "Computer Science",  Code = "CS",  HeadOfDepartment = "Dr. James Mwangi" },
                new() { Name = "Business Studies",  Code = "BS",  HeadOfDepartment = "Dr. Grace Wanjiku" },
                new() { Name = "Engineering",       Code = "ENG", HeadOfDepartment = "Dr. Peter Kamau"  },
                new() { Name = "Health Sciences",   Code = "HS",  HeadOfDepartment = "Dr. Mary Akinyi"  },
            };
            db.Departments.AddRange(depts);
            await db.SaveChangesAsync();

            // ── Courses ────────────────────────────────────────────────────────
            var courses = new List<Course>
            {
                new() { Name = "Bachelor of Computer Science", Code = "BCS",  Credits = 120, DurationYears = 4, DepartmentId = depts[0].Id },
                new() { Name = "Diploma in IT",                Code = "DIT",  Credits = 72,  DurationYears = 2, DepartmentId = depts[0].Id },
                new() { Name = "Bachelor of Commerce",         Code = "BCOM", Credits = 120, DurationYears = 4, DepartmentId = depts[1].Id },
                new() { Name = "Bachelor of Engineering",      Code = "BENG", Credits = 160, DurationYears = 5, DepartmentId = depts[2].Id },
                new() { Name = "Bachelor of Nursing",          Code = "BNS",  Credits = 140, DurationYears = 4, DepartmentId = depts[3].Id },
            };
            db.Courses.AddRange(courses);
            await db.SaveChangesAsync();

            // ── Subjects ───────────────────────────────────────────────────────
            var subjects = new List<Subject>
            {
                new() { Name = "Introduction to Programming", Code = "CS101", Credits = 3, YearOfStudy = 1, Semester = "Semester 1", CourseId = courses[0].Id },
                new() { Name = "Data Structures",             Code = "CS201", Credits = 3, YearOfStudy = 1, Semester = "Semester 2", CourseId = courses[0].Id },
                new() { Name = "Database Systems",            Code = "CS301", Credits = 3, YearOfStudy = 2, Semester = "Semester 1", CourseId = courses[0].Id },
                new() { Name = "Web Development",             Code = "CS401", Credits = 3, YearOfStudy = 2, Semester = "Semester 2", CourseId = courses[0].Id },
                new() { Name = "Business Mathematics",        Code = "BC101", Credits = 3, YearOfStudy = 1, Semester = "Semester 1", CourseId = courses[2].Id },
                new() { Name = "Financial Accounting",        Code = "BC201", Credits = 3, YearOfStudy = 1, Semester = "Semester 2", CourseId = courses[2].Id },
            };
            db.Subjects.AddRange(subjects);
            await db.SaveChangesAsync();

            // ── Students ───────────────────────────────────────────────────────
            var random  = new Random(42);
            var names   = new[] {
                ("Kamau","Wanjiku"), ("Otieno","Akinyi"), ("Njoroge","Muthoni"), ("Kipchoge","Chebet"),
                ("Mwangi","Grace"),  ("Hassan","Fatuma"), ("Mutua","Wambui"),    ("Kariuki","Njeri"),
                ("Gitonga","Mercy"), ("Omondi","Adhiambo"), ("Ndegwa","Esther"), ("Koech","Chelangat"),
            };

            var students = new List<Student>();
            int seq = 1;
            foreach (var (first, last) in names)
            {
                var course = courses[random.Next(courses.Count)];
                students.Add(new Student
                {
                    FirstName       = first,
                    LastName        = last,
                    Email           = $"{first.ToLower()}.{last.ToLower()}@student.school.com",
                    PhoneNumber     = $"07{random.Next(10, 99)}{random.Next(100000, 999999)}",
                    DateOfBirth     = new DateTime(2000 + random.Next(0, 5), random.Next(1, 12), random.Next(1, 28)),
                    AdmissionNumber = $"STU/{DateTime.Now.Year}/{seq++:D4}",
                    EnrollmentDate  = DateTime.Now.AddDays(-random.Next(30, 365)),
                    Gender          = seq % 2 == 0 ? "Female" : "Male",
                    IsActive        = true,
                    CourseId        = course.Id,
                });
            }
            db.Students.AddRange(students);
            await db.SaveChangesAsync();

            // ── Grades (sample) ────────────────────────────────────────────────
            foreach (var student in students)
            {
                var studentSubjects = subjects.Where(s => s.CourseId == student.CourseId).ToList();
                foreach (var subject in studentSubjects)
                {
                    var score = (decimal)(random.Next(40, 100));
                    db.Grades.Add(new Grade
                    {
                        StudentId   = student.Id,
                        SubjectId   = subject.Id,
                        Score       = score,
                        LetterGrade = Grade.ComputeLetter(score),
                        Points      = Grade.ComputePoints(score),
                        Semester    = subject.Semester,
                        Year        = DateTime.Now.Year,
                    });
                }
            }

            // ── Fee Payments (sample) ──────────────────────────────────────────
            int receiptSeq = 1;
            foreach (var student in students)
            {
                db.FeePayments.Add(new FeePayment
                {
                    StudentId         = student.Id,
                    ReceiptNumber     = $"RCP/{DateTime.Now.Year}/{receiptSeq++:D4}",
                    Amount            = random.Next(15000, 50000),
                    Balance           = random.Next(0, 10000),
                    Term              = "Term 1",
                    Year              = DateTime.Now.Year,
                    PaymentMethod     = new[] { "Mpesa", "Bank", "Cash" }[random.Next(3)],
                    TransactionReference = $"QHG{random.Next(100000000, 999999999)}",
                    PaidOn            = DateTime.Now.AddDays(-random.Next(1, 60)),
                });
            }

            await db.SaveChangesAsync();
        }

        private static async Task EnsureUser(UserManager<IdentityUser> um, string email, string password, string role)
        {
            if (await um.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await um.CreateAsync(user, password);
                if (result.Succeeded)
                    await um.AddToRoleAsync(user, role);
            }
        }
    }
}