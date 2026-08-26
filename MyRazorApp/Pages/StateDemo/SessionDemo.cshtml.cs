using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.StateDemo;

public class SessionDemoModel : PageModel
{
    [BindProperty]
    public string UserNickname { get; set; } = string.Empty;

    [BindProperty]
    public string NewCartItem { get; set; } = string.Empty;

    [TempData]
    public string? FlashMessage { get; set; }

    public string CurrentSessionUser { get; set; } = "Guest";
    public int VisitCount { get; set; }
    public List<string> CartItems { get; set; } = [];
    public string SessionId { get; set; } = string.Empty;

    public void OnGet()
    {
        // 1. Session ID (stored in .AspNetCore.Session cookie)
        SessionId = HttpContext.Session.Id;

        // 2. Increment Visit Count
        int visits = HttpContext.Session.GetInt32("VisitCount") ?? 0;
        visits++;
        HttpContext.Session.SetInt32("VisitCount", visits);
        VisitCount = visits;

        // 3. User Nickname from Session
        CurrentSessionUser = HttpContext.Session.GetString("SessionUser") ?? "Guest";
        UserNickname = CurrentSessionUser;

        // 4. Cart Items from Session (stored as comma-separated string)
        string? cartRaw = HttpContext.Session.GetString("SessionCart");
        if (!string.IsNullOrWhiteSpace(cartRaw))
        {
            CartItems = cartRaw.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }

    public IActionResult OnPostSetUser()
    {
        if (!string.IsNullOrWhiteSpace(UserNickname))
        {
            HttpContext.Session.SetString("SessionUser", UserNickname.Trim());
            FlashMessage = $"Session key 'SessionUser' set to '{UserNickname.Trim()}'!";
        }
        return RedirectToPage("/StateDemo/SessionDemo");
    }

    public IActionResult OnPostAddToCart()
    {
        if (!string.IsNullOrWhiteSpace(NewCartItem))
        {
            string? cartRaw = HttpContext.Session.GetString("SessionCart") ?? "";
            var items = cartRaw.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
            items.Add(NewCartItem.Trim());
            HttpContext.Session.SetString("SessionCart", string.Join("|", items));
            HttpContext.Session.SetInt32("SessionCartCount", items.Count);

            FlashMessage = $"Added '{NewCartItem.Trim()}' to Server Session Cart!";
        }
        return RedirectToPage("/StateDemo/SessionDemo");
    }

    public IActionResult OnPostClearSession()
    {
        HttpContext.Session.Clear();
        FlashMessage = "Server Session cleared via HttpContext.Session.Clear()!";
        return RedirectToPage("/StateDemo/SessionDemo");
    }
}
