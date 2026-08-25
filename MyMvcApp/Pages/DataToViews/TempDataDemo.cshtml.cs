using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyMvcApp.Pages.DataToViews;

public class TempDataDemoModel : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPostSetTempDataMessage()
    {
        StatusMessage = $"Staff record updated successfully at {DateTime.Now:T}.";
        return RedirectToPage();
    }
}
