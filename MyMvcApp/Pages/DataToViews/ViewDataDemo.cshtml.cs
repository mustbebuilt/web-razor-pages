using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyMvcApp.Pages.DataToViews;

public class ViewDataDemoModel : PageModel
{
    public void OnGet()
    {
        ViewData["Title"] = "ViewData Example (Staff)";
        ViewData["StaffCount"] = 3;
        ViewData["Items"] = new List<StaffSummary>
        {
            new() { Id = 1, Name = "Alice Johnson", Department = "Engineering", JobTitle = "Manager" },
            new() { Id = 2, Name = "Brian Lee", Department = "Engineering", JobTitle = "Senior Developer" },
            new() { Id = 3, Name = "Carla Gomez", Department = "Human Resources", JobTitle = "Specialist" }
        };
    }
}
