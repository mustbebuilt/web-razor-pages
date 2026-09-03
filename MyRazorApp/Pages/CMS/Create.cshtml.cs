using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;

namespace MyRazorApp.Pages.CMS;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public StaffInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Staff.Add(Input.ToStaff());
        await _context.SaveChangesAsync();
        TempData["Message"] = "Staff record created.";

        return RedirectToPage("Index");
    }
}