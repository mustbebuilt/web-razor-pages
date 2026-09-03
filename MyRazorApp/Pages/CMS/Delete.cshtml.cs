using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.CMS;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Models.Staff Staff { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var staff = await _context.Staff.AsNoTracking().FirstOrDefaultAsync(item => item.StaffId == id);
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
        if (staff == null)
        {
            return NotFound();
        }

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
        TempData["Message"] = "Staff record deleted.";

        return RedirectToPage("Index");
    }
}