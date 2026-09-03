# Deleting Records in Razor Pages (Delete Operation)

## Overview: Safe Deletion & The Confirmation Pattern

The **Delete** operation permanently removes a record from the database.

> [!WARNING]
> **Never delete data on an HTTP GET request!**
> If deletion is triggered by a GET link (e.g. `<a href="/CMS/Delete/5">`), search engine crawlers, browser link pre-fetchers, or accidental clicks could delete your entire database without confirmation.

In ASP.NET Core Razor Pages, safe deletion follows a **Two-Step Confirmation Flow**:
1. **HTTP GET (`OnGetAsync`)**: Navigates to a confirmation screen displaying a summary of the record to be deleted.
2. **HTTP POST (`OnPostAsync`)**: The user clicks a "Delete record" button inside an HTML form to explicitly confirm and execute the deletion.

---

## 1. Route Templates & Constraints

Like the Edit page, the Delete page uses the integer route constraint `@page "{id:int}"` at the top of `Pages/CMS/Delete.cshtml`.

---

## 2. The PageModel (`Pages/CMS/Delete.cshtml.cs`)

Here is the complete C# `DeleteModel` class:

```csharp
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

    // 1. GET: Fetch record details to show in the confirmation summary
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

    // 2. POST: Execute permanent record deletion
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
```

---

## 3. Step-by-Step Execution Lifecycle

```
======================== HTTP GET (/CMS/Delete/3) ========================
Browser sends GET /CMS/Delete/3
           │
           ▼
    OnGetAsync(3)
           │
           ├─► _context.Staff.AsNoTracking().FirstOrDefaultAsync(item => item.StaffId == 3)
           │      │
           │      ├─► null? ──────► return NotFound() (404)
           │      └─► found?
           │            │
           ▼            ▼
   Staff = staff
   return Page()  ──► Renders confirmation prompt with read-only record summary


======================== HTTP POST (/CMS/Delete/3) =======================
User clicks red "Delete record" confirmation button
           │
           ▼
    OnPostAsync(3)
           │
           ├─► _context.Staff.FindAsync(3)
           │      │
           │      ├─► null? ──────► return NotFound() (404)
           │      └─► found?
           │            │
           ▼            ▼
   _context.Staff.Remove(staff)
   await _context.SaveChangesAsync()  (Generates SQL DELETE statement)
           │
           ▼
   TempData["Message"] = "Staff record deleted."
   return RedirectToPage("Index")
```

### Key Technical Details
- **`AsNoTracking()` in `OnGetAsync`**: The GET request only reads data to render the confirmation screen, so entity tracking is disabled for optimal performance.
- **`_context.Staff.Remove(staff)` in `OnPostAsync`**: Marks the entity in the `EntityState.Deleted` state.
- **`await _context.SaveChangesAsync()`**: Executes the SQL `DELETE FROM Staff WHERE StaffId = @id;` command in the database.
- **`TempData["Message"]`**: Transmits the confirmation notice to the `/CMS/Index` dashboard.

---

## 4. The Razor View (`Pages/CMS/Delete.cshtml`)

The view displays the record details inside a semantic HTML Definition List (`<dl>`), followed by a POST form:

```cshtml
@page "{id:int}"
@model MyRazorApp.Pages.CMS.DeleteModel
@{
    ViewData["Title"] = "Delete Staff Member";
}

<h1>Delete Staff Member</h1>
<p>Are you sure you want to delete this staff record?</p>

<dl>
    <dt>Name</dt>
    <dd>@Model.Staff.FirstName @Model.Staff.LastName</dd>
    <dt>Email</dt>
    <dd>@Model.Staff.Email</dd>
    <dt>Department</dt>
    <dd>@Model.Staff.Department</dd>
</dl>

<form method="post">
    <button type="submit" class="btn btn-danger">Delete record</button>
    <a asp-page="Index">Cancel</a>
</form>
```

---

## 5. Summary of the Complete CRUD Workflow

With the creation of these pages, our application features a complete, secure CRUD implementation:

| Action | Route | Method | PageModel | Key EF Core Method |
| :--- | :--- | :--- | :--- | :--- |
| **List** | `/CMS/Index` | `GET` | `IndexModel` | `_context.Staff.AsNoTracking().ToListAsync()` |
| **Create** | `/CMS/Create` | `GET` / `POST` | `CreateModel` | `_context.Staff.Add(Input.ToStaff())` |
| **Edit** | `/CMS/Edit/{id:int}` | `GET` / `POST` | `EditModel` | `Input.ApplyTo(staff)` / `SaveChangesAsync()` |
| **Delete** | `/CMS/Delete/{id:int}` | `GET` / `POST` | `DeleteModel` | `_context.Staff.Remove(staff)` |

All operations follow standard web architecture:
- Separation of input models and database entities.
- Comprehensive client-side and server-side validation.
- Safe HTTP methods (GET for safe reads, POST for state mutations).
- Post-Redirect-Get pattern with flash message alerts.
