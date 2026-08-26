using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.HttpDemo;

public class MultiRouteDemoModel : PageModel
{
    // Bound from route path segment 1: @page "{department?}/{id:int?}"
    [BindProperty(SupportsGet = true)]
    public string? Department { get; set; }

    // Bound from route path segment 2: @page "{department?}/{id:int?}"
    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    // Bound from query string ?tab=...
    [BindProperty(SupportsGet = true)]
    public string? Tab { get; set; } = "overview";

    public List<DepartmentGroup> AvailableDepartments { get; set; } = [];
    public StaffMemberDemo? SelectedMember { get; set; }
    public List<StaffMemberDemo> DepartmentStaff { get; set; } = [];

    private static readonly List<StaffMemberDemo> StaffDatabase =
    [
        new(101, "Alice Morgan", "Lead Software Engineer", "engineering", "alice@example.com", "£75,000", "Full-Time", "2019-03-15"),
        new(102, "Bob Chen", "Senior UX Designer", "design", "bob@example.com", "£68,000", "Full-Time", "2020-07-01"),
        new(103, "Carla Santos", "DevOps Specialist", "engineering", "carla@example.com", "£72,000", "Full-Time", "2021-11-10"),
        new(104, "David Kim", "Product Manager", "product", "david@example.com", "£80,000", "Full-Time", "2018-01-22"),
        new(105, "Eva Green", "Frontend Developer", "engineering", "eva@example.com", "£64,000", "Full-Time", "2022-04-12"),
        new(106, "Frank Wright", "UI/UX Researcher", "design", "frank@example.com", "£60,000", "Full-Time", "2021-09-01")
    ];

    public void OnGet(string? department, int? id)
    {
        AvailableDepartments =
        [
            new("engineering", "Engineering (3 staff)"),
            new("design", "Design (2 staff)"),
            new("product", "Product (1 staff)")
        ];

        if (string.IsNullOrWhiteSpace(Department))
        {
            Department = "engineering"; // Default department route parameter
        }

        DepartmentStaff = StaffDatabase
            .Where(s => s.Department.Equals(Department, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (id.HasValue)
        {
            SelectedMember = DepartmentStaff.FirstOrDefault(s => s.Id == id.Value)
                           ?? StaffDatabase.FirstOrDefault(s => s.Id == id.Value);
        }
        else if (DepartmentStaff.Any())
        {
            SelectedMember = DepartmentStaff.First();
            Id = SelectedMember.Id;
        }
    }
}

public record DepartmentGroup(string Slug, string DisplayName);
