using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;

namespace MyMvcApp.Pages.Staff;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Models.Staff Staff { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null)
        {
            return NotFound();
        }

        Staff = staff;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff != null)
        {
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            TempData["StatusMessage"] = $"Staff member {staff.FirstName} {staff.LastName} deleted successfully!";
        }

        return RedirectToPage("/Staff/Index");
    }
}
