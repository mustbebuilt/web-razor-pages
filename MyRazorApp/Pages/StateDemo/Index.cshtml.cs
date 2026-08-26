using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.StateDemo;

public class IndexModel : PageModel
{
    public string ActiveCookieTheme { get; set; } = "light";
    public string? ActiveSessionUser { get; set; }
    public int ActiveSessionCartCount { get; set; }

    public void OnGet()
    {
        ActiveCookieTheme = Request.Cookies["UserTheme"] ?? "light";
        ActiveSessionUser = HttpContext.Session.GetString("SessionUser");
        ActiveSessionCartCount = HttpContext.Session.GetInt32("SessionCartCount") ?? 0;
    }
}
