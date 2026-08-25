# Forms, Validation, and Full CRUD Operations in Razor Pages

## Form Handling & Model Binding in Razor Pages

In ASP.NET Core Razor Pages, form handling revolves around HTTP `POST` handler methods (`OnPost()`, `OnPostAsync()`) and the `[BindProperty]` attribute.

Instead of passing individual parameter arguments to handler methods, `[BindProperty]` binds submitted HTML form fields directly to C# properties defined on the `PageModel`.

---

## 1. Data Annotations for Form Validation

Validation rules are declared directly on entity or model properties using C# **Data Annotations** (found in `System.ComponentModel.DataAnnotations`).

Example entity class (`Models/Staff.cs`):

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models;

public class Staff
{
    public int StaffId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Salary is required.")]
    [Range(10000, 500000, ErrorMessage = "Salary must be between £10,000 and £500,000.")]
    public double Salary { get; set; }

    public int IsActive { get; set; } = 1;
}
```

---

## 2. Server-Side Validation (`ModelState.IsValid`)

When a user submits a form, ASP.NET Core evaluates data annotation rules against the bound properties. The handler method checks `ModelState.IsValid` before updating the database:

```csharp
// File: Pages/Staff/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Staff;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Models.Staff Staff { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        // 1. Check if model validation rules pass
        if (!ModelState.IsValid)
        {
            // Re-render page with validation error messages
            return Page();
        }

        // 2. Save new record to database
        _context.Staff.Add(Staff);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = $"Staff member {Staff.FirstName} {Staff.LastName} created successfully!";
        
        // 3. POST-Redirect-GET pattern
        return RedirectToPage("/Staff/Index");
    }
}
```

---

## 3. Rendering Form Controls and Validation Tag Helpers

In Razor views, form input fields use `asp-for` Tag Helpers to automatically generate HTML `name`, `id`, `value`, and `data-val` attributes matching the `PageModel` properties:

```cshtml
@page
@model MyRazorApp.Pages.Staff.CreateModel

<form method="post">
    <!-- Validation Summary for overall model errors -->
    <div asp-validation-summary="ModelOnly"></div>

    <div>
        <label asp-for="Staff.FirstName">First Name</label>
        <input asp-for="Staff.FirstName" />
        <!-- Field-level validation error message -->
        <span asp-validation-for="Staff.FirstName"></span>
    </div>

    <div>
        <label asp-for="Staff.Email">Email Address</label>
        <input asp-for="Staff.Email" type="email" />
        <span asp-validation-for="Staff.Email"></span>
    </div>

    <button type="submit">Create</button>
</form>

<!-- Include client-side validation script partial -->
@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

> [!TIP]
> Including `<partial name="_ValidationScriptsPartial" />` enables jQuery Unobtrusive Validation in the browser. Form fields are validated instantly as the user types without waiting for a full server roundtrip!

---

## 4. Complete CRUD Operations Overview

A complete CRUD feature set in Razor Pages consists of 4 core page workflows:

### A. Read (List & Filter) — `Pages/Staff/Index.cshtml`
- Uses `[BindProperty(SupportsGet = true)]` to handle search queries (`SearchTerm`) and dropdown filters (`DepartmentFilter`).

### B. Create — `Pages/Staff/Create.cshtml`
- Displays an empty form (`OnGet()`) and processes creation in `OnPostAsync()`.

### C. Update (Edit) — `Pages/Staff/Edit.cshtml`
- Uses a route parameter `@page "{id:int}"` to load existing entity data in `OnGetAsync(int id)` and updates the record in `OnPostAsync()`.

```csharp
public async Task<IActionResult> OnGetAsync(int id)
{
    var staff = await _context.Staff.FindAsync(id);
    if (staff == null) return NotFound();

    Staff = staff;
    return Page();
}
```

### D. Delete — `Pages/Staff/Delete.cshtml`
- Uses `@page "{id:int}"` to display a confirmation summary and performs deletion in `OnPostAsync(int id)`:

```csharp
public async Task<IActionResult> OnPostAsync(int id)
{
    var staff = await _context.Staff.FindAsync(id);
    if (staff != null)
    {
        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = "Staff member deleted successfully.";
    }
    return RedirectToPage("/Staff/Index");
}
```
