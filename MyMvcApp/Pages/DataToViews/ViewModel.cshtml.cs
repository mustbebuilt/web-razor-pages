using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyMvcApp.Pages.DataToViews;

public class ViewModelModel : PageModel
{
    public string PageTitle { get; set; } = string.Empty;
    public List<StaffSummary> StaffMembers { get; set; } = [];

    public void OnGet()
    {
        PageTitle = "Staff Directory (Strongly-Typed PageModel Data)";
        StaffMembers =
        [
            new StaffSummary { Id = 1, Name = "Alice Johnson", Department = "Engineering", JobTitle = "Manager" },
            new StaffSummary { Id = 2, Name = "Brian Lee", Department = "Engineering", JobTitle = "Senior Developer" },
            new StaffSummary { Id = 3, Name = "Carla Gomez", Department = "Human Resources", JobTitle = "Specialist" }
        ];
    }
}

public class StaffSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
}
