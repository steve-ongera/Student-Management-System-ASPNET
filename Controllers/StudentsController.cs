// Controllers/StudentsController.cs
public class StudentsController : Controller
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db) => _db = db;

    // GET: /Students
    public async Task<IActionResult> Index(string search, int? courseId)
    {
        var query = _db.Students
            .Include(s => s.Course)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(s =>
                s.FirstName.Contains(search) ||
                s.LastName.Contains(search) ||
                s.AdmissionNumber.Contains(search));

        if (courseId.HasValue)
            query = query.Where(s => s.CourseId == courseId);

        ViewBag.Courses = await _db.Courses.ToListAsync();
        return View(await query.ToListAsync());
    }

    // GET: /Students/Create
    public IActionResult Create()
    {
        ViewBag.Courses = new SelectList(_db.Courses, "Id", "Name");
        return View();
    }

    // POST: /Students/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Courses = new SelectList(_db.Courses, "Id", "Name");
            return View(student);
        }

        student.AdmissionNumber = GenerateAdmissionNumber();
        student.EnrollmentDate = DateTime.Now;
        _db.Students.Add(student);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Student registered successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Students/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student == null) return NotFound();
        ViewBag.Courses = new SelectList(_db.Courses, "Id", "Name", student.CourseId);
        return View(student);
    }

    // POST: /Students/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id) return BadRequest();
        if (!ModelState.IsValid) return View(student);

        _db.Update(student);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Student updated.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Students/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student != null) _db.Students.Remove(student);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private string GenerateAdmissionNumber()
    {
        var year = DateTime.Now.Year;
        var count = _db.Students.Count() + 1;
        return $"STU/{year}/{count:D4}";
    }
}