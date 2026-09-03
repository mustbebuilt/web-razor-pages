# Creating Records in Razor Pages (Create Operation)

## Overview: The Create Workflow

The **Create** operation allows users to enter new data through an HTML form and save it to the database.

In ASP.NET Core Razor Pages, implementing a create workflow requires:
1. An **HTTP GET** request to display an empty input form (`Create.cshtml`).
2. Two-way **Model Binding** via `[BindProperty]` on the `PageModel`.
3. An **HTTP POST** handler method (`OnPostAsync`) to validate user input, map it to a database entity, and commit changes using Entity Framework Core.
4. Implementing the **Post-Redirect-Get (PRG)** pattern to prevent duplicate form submissions.

---

## 1. The PageModel (`Pages/CMS/Create.cshtml.cs`)

Here is the complete C# `CreateModel` class:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Data;

namespace MyRazorApp.Pages.CMS;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Two-way binding for form submission
    [BindProperty]
    public StaffInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        // 1. Verify that all Data Annotation validation rules passed
        if (!ModelState.IsValid)
        {
            // Re-render the page to display validation error messages
            return Page();
        }

        // 2. Map input model to a new entity and insert into database
        _context.Staff.Add(Input.ToStaff());
        await _context.SaveChangesAsync();

        // 3. Set temporary flash feedback message
        TempData["Message"] = "Staff record created.";

        // 4. Follow Post-Redirect-Get (PRG) pattern
        return RedirectToPage("Index");
    }
}
```

---

## 2. Step-by-Step Breakdown of `OnPostAsync`

```
 User clicks "Create record" button
                │
                ▼
      [HTTP POST Submission]
                │
                ▼
     ┌──────────────────────┐
     │  ModelState.IsValid? │
     └──────────┬───────────┘
          No    │    Yes
      ┌─────────┴─────────┐
      ▼                   ▼
 return Page()       _context.Staff.Add(Input.ToStaff())
(Shows errors)       await _context.SaveChangesAsync()
                          │
                          ▼
                     TempData["Message"] = "..."
                     return RedirectToPage("Index")
```

### 1. `[BindProperty]`
By default, Razor Pages only binds HTTP GET route and query parameters. Applying `[BindProperty]` instructs ASP.NET Core to automatically deserialize HTTP POST form payload values into the `Input` instance before calling `OnPostAsync()`.

### 2. Validation Check (`ModelState.IsValid`)
- If any required field is missing or invalid (e.g. malformed email or negative salary), `ModelState.IsValid` returns `false`.
- Returning `Page()` re-renders `Create.cshtml` with the user's entered data preserved and error messages displayed beside each invalid field.

### 3. Entity Insertion & Persistence
- `Input.ToStaff()` converts the sanitized input values into a new `Models.Staff` entity.
- `_context.Staff.Add(...)` attaches the entity in the `EntityState.Added` state.
- `await _context.SaveChangesAsync()` generates and executes the database SQL `INSERT` statement asynchronously.

### 4. Post-Redirect-Get (PRG) Pattern
Returning `RedirectToPage("Index")` sends an HTTP `302 Found` redirect response back to the client, redirecting the browser to `/CMS/Index`. This prevents duplicate form submissions if the user refreshes the page or clicks the back button.

---

## 3. The Razor View (`Pages/CMS/Create.cshtml`)

The view includes the shared form partial view and submit controls:

```cshtml
@page
@model MyRazorApp.Pages.CMS.CreateModel
@{
    ViewData["Title"] = "Add Staff Member";
}

<h1>Add Staff Member</h1>
<p>Enter the details for a new staff record.</p>

<form method="post">
    <!-- Shared input form partial -->
    <partial name="_StaffForm" model="Model.Input" />

    <button type="submit" class="btn btn-primary">Create record</button>
    <a asp-page="Index">Cancel</a>
</form>
```

> [!TIP]
> In the next tutorial, **`09e-crud-edit.md`**, we will build the **Edit** page to load an existing record by route ID, display its current values in the form, and save modifications.
