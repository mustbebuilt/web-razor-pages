using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Staff;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Models.Staff> StaffMembers { get; set; } = [];
    public int ActiveCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Staff.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm.Trim().ToLower();
            query = query.Where(s => s.LastName.ToLower().Contains(term));
        }

        StaffMembers = await query.ToListAsync();
        ActiveCount = StaffMembers.Count(s => s.IsActive == 1);
    }
}