using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.StateDemo;

public class CookieDemoModel : PageModel
{
    [BindProperty]
    public string SelectedTheme { get; set; } = "light";

    [BindProperty]
    public bool IsPersistent { get; set; } = true;

    [TempData]
    public string? FlashMessage { get; set; }

    public string CurrentCookieTheme { get; set; } = "light";
    public List<KeyValuePair<string, string>> AllCookies { get; set; } = [];

    public void OnGet()
    {
        CurrentCookieTheme = Request.Cookies["UserTheme"] ?? "light";
        SelectedTheme = CurrentCookieTheme;

        // Read all active cookies sent in Request.Cookies
        AllCookies = Request.Cookies
            .Select(c => new KeyValuePair<string, string>(c.Key, c.Value))
            .ToList();
    }

    public IActionResult OnPostSetTheme()
    {
        var options = new CookieOptions
        {
            HttpOnly = false,
            IsEssential = true
        };

        if (IsPersistent)
        {
            options.Expires = DateTimeOffset.UtcNow.AddDays(7); // 7-day persistent cookie
        }

        Response.Cookies.Append("UserTheme", SelectedTheme, options);
        FlashMessage = $"Cookie 'UserTheme' updated to '{SelectedTheme}' ({(IsPersistent ? "Persistent 7 Days" : "Session Cookie")})!";

        return RedirectToPage("/StateDemo/CookieDemo");
    }

    public IActionResult OnPostDeleteCookie()
    {
        Response.Cookies.Delete("UserTheme");
        FlashMessage = "Cookie 'UserTheme' deleted! Theme reset to default Light.";

        return RedirectToPage("/StateDemo/CookieDemo");
    }
}
