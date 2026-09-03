# Updating Records in Razor Pages (Edit Operation)

## Overview: The Edit Workflow

The **Edit / Update** operation modifies an existing record in the database. 

Unlike creating a new record, the edit lifecycle involves two distinct database interactions:
1. **HTTP GET (`OnGetAsync`)**: Reads the record from the database by its primary key ID and populates the form controls with the existing values.
2. **HTTP POST (`OnPostAsync`)**: Validates the submitted form, fetches the tracked database entity, applies the modified properties, and commits the changes using EF Core.

---

## 1. Route Templates & Constraints

The Edit page must know *which* record to edit. We pass the record ID in the URL path (e.g. `/CMS/Edit/3`).

At the top of `Pages/CMS/Edit.cshtml`, the route template is defined with an integer constraint:

```cshtml
@page "{id:int}"
@model MyRazorApp.Pages.CMS.EditModel
```

> [!NOTE]
> The `{id:int}` constraint ensures that navigating to `/CMS/Edit/abc` immediately returns an HTTP `404 Not Found` without executing application code or throwing casting exceptions.

---

## 2. The PageModel (`Pages/CMS/Edit.cshtml.cs`)

Here is the complete C# `EditModel` class:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

namespace MyRazorApp.Pages.CMS;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Two-way binding for form inputs
    [BindProperty]
    public StaffInput Input { get; set; } = new();

    // Binds route parameter {id:int} from URL
    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    // 1. GET: Fetch existing record and pre-populate form
    public async Task<IActionResult> OnGetAsync()
    {
        var staff = await _context.Staff.FindAsync(Id);
        if (staff == null)
        {
            return NotFound();
        }

        // Map entity values to the input model
        Input = new StaffInput(staff);
        return Page();
    }

    // 2. POST: Validate and update existing record
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Query the existing entity to be updated
        var staff = await _context.Staff.FirstOrDefaultAsync(item => item.StaffId == Id);
        if (staff == null)
        {
            return NotFound();
        }

        // Apply updated values onto the tracked EF Core entity
        Input.ApplyTo(staff);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Staff record updated.";
        return RedirectToPage("Index");
    }
}
```

---

## 3. Step-by-Step Execution Lifecycle

```
========================= HTTP GET (/CMS/Edit/3) =========================
Browser sends GET /CMS/Edit/3
           │
           ▼
    OnGetAsync()
           │
           ├─► _context.Staff.FindAsync(3)
           │      │
           │      ├─► null? ──────► return NotFound() (404)
           │      └─► found?
           │            │
           ▼            ▼
   Input = new StaffInput(staff)
           │
           ▼
     return Page()  ──► Renders form pre-populated with Staff #3 values


========================= HTTP POST (/CMS/Edit/3) ========================
User edits values & clicks "Save changes"
           │
           ▼
    OnPostAsync()
           │
           ├─► !ModelState.IsValid? ──► return Page() (re-renders with errors)
           │
           ▼
   _context.Staff.FirstOrDefaultAsync(item => item.StaffId == 3)
           │
           ├─► null? ──► return NotFound() (404)
           │
           ▼
   Input.ApplyTo(staff)
   await _context.SaveChangesAsync()  (Generates SQL UPDATE statement)
           │
           ▼
   TempData["Message"] = "Staff record updated."
   return RedirectToPage("Index")
```

---

## 4. Why Use `ApplyTo(...)` on a Tracked Entity?

In Entity Framework Core, when you retrieve an entity using `FirstOrDefaultAsync` or `FindAsync` (without `AsNoTracking()`), EF Core begins tracking the entity's property values.

When `Input.ApplyTo(staff)` mutates properties on that instance, EF Core detects the changes (e.g. `FirstName` changed from "Jane" to "Janet"). Calling `SaveChangesAsync()` then automatically generates an optimized SQL `UPDATE` statement updating only the modified columns.

---

## 5. The Razor View (`Pages/CMS/Edit.cshtml`)

The view renders the form heading, the shared `_StaffForm` partial, and action buttons:

```cshtml
@page "{id:int}"
@model MyRazorApp.Pages.CMS.EditModel
@{
    ViewData["Title"] = "Edit Staff Member";
}

<h1>Edit Staff Member</h1>
<p>Update the details for staff record #@Model.Id.</p>

<form method="post">
    <!-- Reusable partial view pre-populated with Model.Input -->
    <partial name="_StaffForm" model="Model.Input" />

    <button type="submit" class="btn btn-primary">Save changes</button>
    <a asp-page="Index">Cancel</a>
</form>
```

> [!TIP]
> In the next tutorial, **`09f-crud-delete.md`**, we will build the **Delete** page using a safe two-step confirmation pattern.
