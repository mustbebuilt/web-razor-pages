# Passing Parameters in Razor Pages (Staff Details Page)

## Overview: Route Parameters in Razor Pages

When navigating between pages in a web application, you often need to pass data identifiers (such as a database record ID) to specify which item to display.

In ASP.NET Core Razor Pages, passing parameters between pages is achieved using two complementary features:
1. **The `asp-route-{name}` Tag Helper**: Generates links containing URL route parameters (e.g. `<a asp-page="/Staff/Details" asp-route-id="3">Details</a>`).
2. **The `@page "{id:int}"` Directive**: Defines the URL route template on the target page and constrains the parameter type.

This guide demonstrates creating a **Staff Details** page (`Pages/Staff/Details.cshtml`) linked directly from the Staff Index directory.

---

## 1. Generating Parameter Links in the List Page (`Pages/Staff/Index.cshtml`)

In your staff list table, use the `asp-route-id` Tag Helper on each row link to pass the specific staff member's ID:

```cshtml
@foreach (var member in Model.StaffMembers)
{
    <tr>
        <td>@member.StaffId</td>
        <td>
            <!-- Option A: Link the staff member's name -->
            <a asp-page="/Staff/Details" asp-route-id="@member.StaffId">
                @member.FirstName @member.LastName
            </a>
        </td>
        <td>@member.Department</td>
        <td>@member.JobTitle</td>
        <td>
            <!-- Option B: Action link column -->
            <a asp-page="/Staff/Details" asp-route-id="@member.StaffId">Details</a> |
            <a asp-page="/Staff/Edit" asp-route-id="@member.StaffId">Edit</a> |
            <a asp-page="/Staff/Delete" asp-route-id="@member.StaffId">Delete</a>
        </td>
    </tr>
}
```

> [!NOTE]
> The attribute prefix `asp-route-` dynamically appends route parameters to the generated `href` attribute. For example, `asp-route-id="5"` generates `<a href="/Staff/Details/5">Details</a>`.

---

## 2. Defining the Target Route Template (`Pages/Staff/Details.cshtml`)

At the top of the target Razor Page (`Pages/Staff/Details.cshtml`), define the `@page` directive with the parameter constraint:

```cshtml
@page "{id:int}"
@model MyRazorApp.Pages.Staff.DetailsModel
@{
    ViewData["Title"] = $"Staff Details - {Model.Staff.FirstName} {Model.Staff.LastName}";
}

<h1>Staff Details</h1>
<p>Detailed record for @Model.Staff.FirstName @Model.Staff.LastName.</p>

<dl>
    <dt>Staff ID</dt>
    <dd>@Model.Staff.StaffId</dd>

    <dt>Full Name</dt>
    <dd>@Model.Staff.FirstName @Model.Staff.LastName</dd>

    <dt>Email Address</dt>
    <dd>@Model.Staff.Email</dd>

    <dt>Department</dt>
    <dd>@Model.Staff.Department</dd>

    <dt>Job Title</dt>
    <dd>@Model.Staff.JobTitle</dd>

    <dt>Hire Date</dt>
    <dd>@Model.Staff.HireDate</dd>

    <dt>Salary</dt>
    <dd>@Model.Staff.Salary.ToString("C")</dd>

    <dt>Status</dt>
    <dd>
        @if (Model.Staff.IsActive == 1)
        {
            <span>Active</span>
        }
        else
        {
            <span>Inactive</span>
        }
    </dd>
</dl>

<div>
    <a asp-page="/Staff/Edit" asp-route-id="@Model.Staff.StaffId">Edit Record</a> |
    <a asp-page="/Staff/Index">Back to Staff Directory</a>
</div>
```

---

## 3. Capturing Parameters in the Code-Behind (`Pages/Staff/Details.cshtml.cs`)

Inside the code-behind `PageModel`, declare the parameter as an argument in the `OnGetAsync(int id)` handler method:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Staff;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Models.Staff Staff { get; set; } = null!;

    // The parameter 'id' automatically receives the value from the URL route
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);

        // If no matching staff record is found in the database, return a 404 response
        if (staff == null)
        {
            return NotFound();
        }

        Staff = staff;
        return Page();
    }
}
```

---

## 4. Route Template Variations

Razor Pages supports various route parameter options depending on your URL requirements:

| Route Directive | Matching URL | Description |
|---|---|---|
| `@page` | `/Staff/Details?id=5` | Default query string parameter (no route template). |
| `@page "{id}"` | `/Staff/Details/5` | Route parameter `id` (matches any string or number). |
| `@page "{id:int}"` | `/Staff/Details/5` | Type-constrained integer parameter (rejects non-numeric URLs like `/Staff/Details/abc`). |
| `@page "{id:int?}"` | `/Staff/Details` or `/Staff/Details/5` | Optional integer parameter. |

---

## Summary Checklist

1. Use `asp-page="/Staff/Details"` and `asp-route-id="@item.Id"` in the source view to generate the link.
2. Add `@page "{id:int}"` at the top of the destination `.cshtml` file.
3. Accept `int id` as a parameter in `OnGetAsync(int id)` within the code-behind `.cshtml.cs` file.
