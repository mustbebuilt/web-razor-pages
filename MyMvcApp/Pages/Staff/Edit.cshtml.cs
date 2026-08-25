using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;

namespace MyMvcApp.Pages.Staff;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var staffToUpdate = await _context.Staff.FindAsync(Staff.StaffId);
        if (staffToUpdate == null)
        {
            return NotFound();
        }

        staffToUpdate.FirstName = Staff.FirstName;
        staffToUpdate.LastName = Staff.LastName;
        staffToUpdate.Email = Staff.Email;
        staffToUpdate.Department = Staff.Department;
        staffToUpdate.JobTitle = Staff.JobTitle;
        staffToUpdate.Salary = Staff.Salary;
        staffToUpdate.IsActive = Staff.IsActive;

        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Staff member {Staff.FirstName} {Staff.LastName} updated successfully!";
        return RedirectToPage("/Staff/Index");
    }
}
