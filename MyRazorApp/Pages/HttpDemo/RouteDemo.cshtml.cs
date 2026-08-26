using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.HttpDemo;

public class RouteDemoModel : PageModel
{
    // Bound from the URL route path defined in @page "{id:int?}"
    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    // Bound from the URL query string e.g. ?tab=salary&view=detailed
    [BindProperty(SupportsGet = true)]
    public string? Tab { get; set; } = "overview";

    [BindProperty(SupportsGet = true)]
    public string? ViewMode { get; set; } = "standard";

    public StaffMemberDemo? SelectedMember { get; set; }
    public List<StaffMemberDemo> SampleStaff { get; set; } = [];

    private static readonly List<StaffMemberDemo> StaffDatabase =
    [
        new(101, "Alice Morgan", "Lead Software Engineer", "Engineering", "alice@example.com", "£75,000", "Full-Time", "2019-03-15"),
        new(102, "Bob Chen", "Senior UX Designer", "Design", "bob@example.com", "£68,000", "Full-Time", "2020-07-01"),
        new(103, "Carla Santos", "DevOps Specialist", "Engineering", "carla@example.com", "£72,000", "Full-Time", "2021-11-10"),
        new(104, "David Kim", "Product Manager", "Product", "david@example.com", "£80,000", "Full-Time", "2018-01-22")
    ];

    public void OnGet(int? id)
    {
        SampleStaff = StaffDatabase;

        if (id.HasValue)
        {
            SelectedMember = StaffDatabase.FirstOrDefault(s => s.Id == id.Value);
        }
        else
        {
            SelectedMember = StaffDatabase.First(); // Default selection
        }
    }
}

public record StaffMemberDemo(int Id, string Name, string Role, string Department, string Email, string Salary, string EmploymentType, string HiredDate);
