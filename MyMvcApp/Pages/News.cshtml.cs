using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyMvcApp.Pages;

public class NewsModel : PageModel
{
    public List<NewsItem> Articles { get; set; } = [];

    public void OnGet()
    {
        Articles =
        [
            new NewsItem { Id = 1, Title = "ASP.NET Core Razor Pages Tutorial", Summary = "Learn how Razor Pages simplifies building page-focused web apps with code-behind PageModels.", Date = DateTime.Now.AddDays(-2) },
            new NewsItem { Id = 2, Title = "Clean Architecture in .NET 10", Summary = "Discover best practices for organizing modern C# web applications.", Date = DateTime.Now.AddDays(-5) }
        ];
    }
}

public class NewsItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
