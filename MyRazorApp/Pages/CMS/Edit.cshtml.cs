using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

namespace MyRazorApp.Pages.CMS;

public class EditModel : CmsPageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context, CmsAuthentication authentication) : base(authentication)
    {
        _context = context;
    }

    [BindProperty]
    public StaffInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var staff = await _context.Staff.FindAsync(Id);
        if (staff == null)
        {
            return NotFound();
        }

        Input = new StaffInput(staff);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var staff = await _context.Staff.FirstOrDefaultAsync(item => item.StaffId == Id);
        if (staff == null)
        {
            return NotFound();
        }

        Input.ApplyTo(staff);
        await _context.SaveChangesAsync();
        TempData["Message"] = "Staff record updated.";

        return RedirectToPage("Index");
    }
}