using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyMvcApp.Pages;

public class IndexModel : PageModel
{
    public string Message { get; set; } = string.Empty;
    public List<string> FeaturedStaff { get; set; } = [];

    public void OnGet()
    {
        Message = "Welcome to ASP.NET Core Razor Pages";
        FeaturedStaff = ["Alice Johnson (Engineering)", "Brian Lee (Engineering)", "Carla Gomez (HR)", "David Smith (Finance)"];
        ViewData["Message"] = "Hello from ViewData in Index PageModel";
    }
}
