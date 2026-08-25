using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;

namespace MyMvcApp.Pages.Staff;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Models.Staff> StaffList { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? DepartmentFilter { get; set; }

    public List<string> Departments { get; set; } = [];
    public int ActiveCount => StaffList.Count(s => s.IsActive == 1);

    public async Task OnGetAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        var query = _context.Staff.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm.Trim().ToLower();
            query = query.Where(s => s.FirstName.ToLower().Contains(term) ||
                                     s.LastName.ToLower().Contains(term) ||
                                     s.Email.ToLower().Contains(term) ||
                                     s.JobTitle.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(DepartmentFilter))
        {
            query = query.Where(s => s.Department == DepartmentFilter);
        }

        StaffList = query.ToList();
        Departments = _context.Staff.Select(s => s.Department).Distinct().OrderBy(d => d).ToList();

        if (!StaffList.Any() && string.IsNullOrWhiteSpace(SearchTerm) && string.IsNullOrWhiteSpace(DepartmentFilter))
        {
            StaffList =
            [
                new Models.Staff { StaffId = 1, FirstName = "Alice", LastName = "Johnson", JobTitle = "Engineering Manager", Department = "Engineering", Email = "alice.johnson@company.com", IsActive = 1, Salary = 85000 },
                new Models.Staff { StaffId = 2, FirstName = "Brian", LastName = "Lee", JobTitle = "Senior Developer", Department = "Engineering", Email = "brian.lee@company.com", IsActive = 1, Salary = 72000 },
                new Models.Staff { StaffId = 3, FirstName = "Carla", LastName = "Gomez", JobTitle = "HR Specialist", Department = "Human Resources", Email = "carla.gomez@company.com", IsActive = 0, Salary = 58000 }
            ];
        }
    }
}
