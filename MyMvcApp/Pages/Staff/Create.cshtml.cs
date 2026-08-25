using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;

namespace MyMvcApp.Pages.Staff;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Models.Staff Staff { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _context.Database.EnsureCreatedAsync();

        _context.Staff.Add(Staff);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Staff member {Staff.FirstName} {Staff.LastName} added successfully!";
        return RedirectToPage("/Staff/Index");
    }
}
