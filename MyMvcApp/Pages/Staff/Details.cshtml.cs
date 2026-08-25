using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;

namespace MyMvcApp.Pages.Staff;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Models.Staff Staff { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        await _context.Database.EnsureCreatedAsync();

        var staff = await _context.Staff.FindAsync(id);

        if (staff == null)
        {
            return NotFound();
        }

        Staff = staff;
        return Page();
    }
}
