using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.HttpDemo;

public class GetDemoModel : PageModel
{
    // Bind properties from the URL query string using SupportsGet = true
    [BindProperty(SupportsGet = true)]
    public string? Subject { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Level { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    public List<Course> FilteredCourses { get; set; } = [];

    // Mock dataset for demonstration
    private static readonly List<Course> AllCourses =
    [
        new("CS101", "Web Development Fundamentals", "Computer Science", "Beginner"),
        new("CS102", "ASP.NET Core Razor Pages", "Computer Science", "Intermediate"),
        new("CS201", "Advanced C# & Entity Framework", "Computer Science", "Advanced"),
        new("DB101", "Database Systems & SQL", "Data Science", "Beginner"),
        new("DS202", "Data Analysis with Python", "Data Science", "Intermediate"),
        new("SE301", "Software Engineering Architecture", "Software Engineering", "Advanced")
    ];

    public void OnGet()
    {
        var query = AllCourses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(Subject))
        {
            query = query.Where(c => c.Category.Equals(Subject, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(Level))
        {
            query = query.Where(c => c.Level.Equals(Level, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            query = query.Where(c => c.Title.Contains(Keyword, StringComparison.OrdinalIgnoreCase) ||
                                     c.Code.Contains(Keyword, StringComparison.OrdinalIgnoreCase));
        }

        FilteredCourses = query.ToList();
    }
}

public record Course(string Code, string Title, string Category, string Level);
