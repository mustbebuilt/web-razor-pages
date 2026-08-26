# Searching Data in Razor Pages (Staff Search)

## Overview: Search and Filtering in Razor Pages

Searching and filtering data is a fundamental feature in web applications. In ASP.NET Core Razor Pages, implementing a search feature relies on three core concepts:

1. **HTTP GET Form Submission**: Sending user search queries via URL query string parameters (e.g. `/Staff?SearchTerm=johnson`).
2. **GET Model Binding (`[BindProperty(SupportsGet = true)]`)**: Automatically mapping URL query parameters to C# properties on the `PageModel`.
3. **Dynamic Querying with EF Core (`IQueryable<T>`)**: Building database queries dynamically using LINQ and Deferred Execution.

This guide explains how search functionality was added to the **Staff Directory** (`Pages/Staff/Index.cshtml` and `Pages/Staff/Index.cshtml.cs`).

---

## 1. Enabling Model Binding for GET Requests (`Pages/Staff/Index.cshtml.cs`)

By default, the `[BindProperty]` attribute in Razor Pages only binds incoming form data during HTTP **POST** requests. To search or filter using query string parameters in an HTTP **GET** request, you must explicitly set `SupportsGet = true`.

### Declaring the Search Property in the PageModel

```csharp
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

    // SupportsGet = true allows property binding from URL query strings (e.g., ?SearchTerm=johnson)
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
}
```

> [!NOTE]
> Using HTTP **GET** for search forms is standard web design best practice. It ensures search results have unique, bookmarkable URLs (such as `/Staff?SearchTerm=johnson`) that can be shared or navigated back to using browser history.

---

## 2. Dynamic Query Building with Entity Framework Core

Inside the `OnGetAsync()` handler method, Entity Framework Core allows you to construct database queries dynamically using `IQueryable<T>` before sending the query to the database.

### Filtering Data with Deferred Execution

```csharp
public async Task OnGetAsync()
{
    // 1. Initialize an unexecuted query definition
    var query = _context.Staff.AsQueryable();

    // 2. Conditionally apply a filter if a search term was provided
    if (!string.IsNullOrWhiteSpace(SearchTerm))
    {
        var term = SearchTerm.Trim().ToLower();
        query = query.Where(s => s.LastName.ToLower().Contains(term));
    }

    // 3. Execute the SQL query asynchronously against the database
    StaffMembers = await query.ToListAsync();

    // 4. Calculate active count from filtered memory list
    ActiveCount = StaffMembers.Count(s => s.IsActive == 1);
}
```

### Key Query Concepts

- **`AsQueryable()`**: Converts the database set into an `IQueryable<Staff>` expression tree. This allows you to chain further LINQ operators (like `.Where()`) conditionally.
- **Deferred Execution**: Entity Framework Core does not send a query to SQLite or SQL Server when `.AsQueryable()` or `.Where()` is called. The actual database query is only compiled and executed when `await query.ToListAsync()` is called.
- **LINQ Substring Matching (`Contains`)**: EF Core translates `.Contains(term)` into the SQL `LIKE '%term%'` operator. Combining `.ToLower()` ensures case-insensitive matching regardless of database engine default collations.

---

## 3. Constructing the Search UI (`Pages/Staff/Index.cshtml`)

In the Razor view, create an HTML form with `method="get"` and connect the search input field to the `SearchTerm` property using the `asp-for` Tag Helper.

```cshtml
@page
@model MyRazorApp.Pages.Staff.IndexModel
@{
    ViewData["Title"] = "Staff Directory";
}

<div>
    <h1>Staff Directory</h1>
    <p>Total Active Employees: @Model.ActiveCount</p>

    <!-- Search Form using HTTP GET -->
    <form method="get" class="search-form">
        <div class="search-group">
            <input type="text" asp-for="SearchTerm" placeholder="Search by last name..." class="search-input" />
            <button type="submit" class="btn btn-primary">Search</button>

            <!-- Render Clear button only when a filter is currently active -->
            @if (!string.IsNullOrWhiteSpace(Model.SearchTerm))
            {
                <a asp-page="/Staff/Index" class="btn btn-secondary">Clear</a>
            }
        </div>
    </form>

    <!-- Display message when search returns no matching records -->
    @if (!Model.StaffMembers.Any())
    {
        <div class="no-results">
            <p>No staff members found matching "<strong>@Model.SearchTerm</strong>".</p>
        </div>
    }
    else
    {
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Department</th>
                    <th>Job Title</th>
                    <th>Salary</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var member in Model.StaffMembers)
                {
                    <tr>
                        <td>@member.StaffId</td>
                        <td>@member.FirstName @member.LastName</td>
                        <td>@member.Email</td>
                        <td>@member.Department</td>
                        <td>@member.JobTitle</td>
                        <td>@member.Salary.ToString("C", new System.Globalization.CultureInfo("en-GB"))</td>
                        <td>
                            @if (member.IsActive == 1)
                            {
                                <span class="status-active">Active</span>
                            }
                            else
                            {
                                <span class="status-inactive">Inactive</span>
                            }
                        </td>
                        <td>
                            <a asp-page="/Staff/Details" asp-route-id="@member.StaffId">Details</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>
```

### Key View Features

1. **`method="get"`**: Submitting the form appends field inputs to the URL as query parameters (`?SearchTerm=val`) rather than sending data hidden in the request body.
2. **`asp-for="SearchTerm"` Tag Helper**:
   - Generates `name="SearchTerm"` and `id="SearchTerm"` on the `<input>` element.
   - Automatically populates the text box with the current value of `@Model.SearchTerm`, retaining the query in the input box after page reloads.
3. **Clear Button**: Uses a standard Razor Page anchor link (`asp-page="/Staff/Index"`) without route parameters. Clicking it navigates to the base index page, clearing the query string and restoring the unfiltered staff list.
4. **Empty Results State**: `@if (!Model.StaffMembers.Any())` checks if any records were returned and displays user feedback if no matching staff were found.

---

## 4. Comparing Search (GET) vs CRUD Forms (POST)

Understanding when to use HTTP `GET` versus HTTP `POST` is vital when building web forms:

| Feature | Search / Filter Forms (`GET`) | Data Mutation / Action Forms (`POST`) |
|---|---|---|
| **HTTP Method** | `method="get"` | `method="post"` |
| **Model Binding** | `[BindProperty(SupportsGet = true)]` | `[BindProperty]` |
| **Data Payload Location** | URL Query String (`?key=value`) | Request Body |
| **Page Reload / Refresh** | Safe; no prompt | Triggers "Confirm Form Resubmission" prompt |
| **Bookmarking / Links** | Yes, full search results can be bookmarked | No, cannot bookmark form post actions |
| **Primary Use Cases** | Search textboxes, category filters, page numbers | Create, Edit, Delete records |

---

## Summary Checklist

1. Add `[BindProperty(SupportsGet = true)]` to public properties in `PageModel` to capture search inputs from GET requests.
2. Use `method="get"` on HTML search forms.
3. Apply `asp-for="PropertyName"` on input fields to bind inputs and retain user entry values on reload.
4. Use `IQueryable<T>` with `_context.Entity.AsQueryable()` to apply LINQ `.Where()` filters dynamically before calling `.ToListAsync()`.
5. Provide a "Clear" link (`asp-page="/Staff/Index"`) to allow users to easily reset search filters.
