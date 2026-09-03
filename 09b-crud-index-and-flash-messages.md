# Staff CMS Index & Status Flash Messages

## Overview: The CMS Management Dashboard

The **Staff CMS Index** (`/CMS/Index`) serves as the central management dashboard for administrators to view, create, edit, and delete staff records.

This page demonstrates three key Razor Pages concepts:
1. **Efficient Read Queries (`AsNoTracking()`)**: Querying database records quickly without incurring EF Core change-tracking overhead.
2. **Flash Messages via `TempData`**: Passing one-time success alerts across HTTP redirects after Create, Edit, or Delete actions.
3. **Action Tag Helpers**: Generating links to Create, Edit, and Delete pages using `asp-page` and `asp-route-id`.

---

## 1. The PageModel (`Pages/CMS/Index.cshtml.cs`)

The `IndexModel` fetches the list of staff members from the database and retrieves any pending feedback message:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.CMS;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Models.Staff> StaffMembers { get; set; } = [];

    // Reads temporary status message passed from Create, Edit, or Delete actions
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
```

### Key Technical Details
- **`AsNoTracking()`**: Since this page is read-only and does not mutate entities, disabling EF Core's change tracker improves memory efficiency and query execution speed.
- **`OrderBy` and `ThenBy`**: Sorts records alphabetically by `LastName`, then by `FirstName`.
- **`TempData["Message"]`**: Accesses session/cookie-backed flash storage. Once read during rendering, ASP.NET Core marks the key for removal so it won't appear again on subsequent page reloads.

---

## 2. The Razor View (`Pages/CMS/Index.cshtml`)

The view renders the management table, action buttons, and any flash status alert:

```cshtml
@page
@model MyRazorApp.Pages.CMS.IndexModel
@{
    ViewData["Title"] = "Staff CMS";
}

<h1>Staff CMS</h1>
<p>Create, update, and remove staff records.</p>

@* Display flash notification if set by previous action *@
@if (!string.IsNullOrEmpty(Model.Message))
{
    <div class="alert-success">@Model.Message</div>
}

<p><a asp-page="Create" class="btn btn-primary">Add staff member</a></p>

@if (!Model.StaffMembers.Any())
{
    <p>No staff records are available.</p>
}
else
{
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Department</th>
                <th>Email</th>
                <th>Status</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var member in Model.StaffMembers)
            {
                <tr>
                    <td>@member.FirstName @member.LastName</td>
                    <td>@member.Department</td>
                    <td>@member.Email</td>
                    <td>@(member.IsActive == 1 ? "Active" : "Inactive")</td>
                    <td>
                        <a asp-page="Edit" asp-route-id="@member.StaffId">Edit</a> |
                        <a asp-page="Delete" asp-route-id="@member.StaffId">Delete</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}
```

---

## 3. How Action Tag Helpers Work

Razor Tag Helpers simplify linking between pages within the same folder structure:

- **Relative Routing (`asp-page="Create"`)**: Automatically resolves to `/CMS/Create` because the current page is located in `Pages/CMS/`.
- **Route Parameters (`asp-route-id="@member.StaffId"`)**: Appends the primary key ID into the target page's route template. For example, for staff ID `4`, it generates:
  ```html
  <a href="/CMS/Edit/4">Edit</a>
  <a href="/CMS/Delete/4">Delete</a>
  ```

---

## 4. Flash Messages in Action

When a user completes a CRUD operation (such as adding or editing a staff member), the handler sets a message:

```csharp
TempData["Message"] = "Staff record created.";
return RedirectToPage("Index");
```

When the browser follows the redirect to `/CMS/Index`, `Model.Message` reads the value and renders the green `.alert-success` callout banner. Refreshing the browser clears the banner automatically.

> [!TIP]
> In the next tutorial, **`09c-shared-form-partial.md`**, we will build `_StaffForm.cshtml`, a reusable partial view that encapsulates form fields and validation tags shared between Create and Edit pages.
