using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.CMS;

public class IndexModel : CmsPageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context, CmsAuthentication authentication) : base(authentication)
    {
        _context = context;
    }

    public List<Models.Staff> StaffMembers { get; set; } = [];

    public string? Message => TempData["Message"] as string;

    public async Task OnGetAsync()
    {
        StaffMembers = await _context.Staff
            .AsNoTracking()
            .OrderBy(staff => staff.LastName)
            .ThenBy(staff => staff.FirstName)
            .ToListAsync();
    }
}