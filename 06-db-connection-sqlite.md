# Database Integration with SQLite and Entity Framework Core in Razor Pages

## SQLite and Entity Framework Core

Entity Framework Core (EF Core) is an Object-Relational Mapper (ORM) that enables .NET developers to interact with databases using strongly-typed C# objects.

This guide demonstrates connecting an SQLite database (`data/staff.db`) to an ASP.NET Core Razor Pages application. You can find the SQL script to create and seed the staff table in [resources/staff-sqlite.sql](file:///Users/martincooper/Documents/github-demo-sites-for-modules/web-razor-pages/resources/staff-sqlite.sql).

---

## 1. Install SQLite EF Core NuGet Package

Open a terminal inside your project directory and add the EF Core SQLite provider:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

---

## 2. Create the Staff Entity Model (`Models/Staff.cs`)

Create a class inside `Models/Staff.cs` matching your database schema:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models;

public class Staff
{
    [Key]
    [Column("staff_id")]
    public int StaffId { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("department")]
    public string Department { get; set; } = string.Empty;

    [Column("job_title")]
    public string JobTitle { get; set; } = string.Empty;

    [Column("hire_date")]
    public string HireDate { get; set; } = string.Empty;

    [Column("salary")]
    public double Salary { get; set; }

    [Column("is_active")]
    public int IsActive { get; set; } // 0 or 1
}
```

---

## 3. Create the Database Context (`Data/ApplicationDbContext.cs`)

The `DbContext` represents a session with the SQLite database:

```csharp
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Staff> Staff { get; set; } = null!;
}
```

---

## 4. Configure Connection String and Register Service

In `appsettings.json`, specify your SQLite connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=data/staff.db;"
  }
}
```

Register `ApplicationDbContext` in `Program.cs` before `builder.Build()`:

```csharp
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Register SQLite DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
```

---

## 5. Inject DbContext into the Razor PageModel (`Pages/Staff/Index.cshtml.cs`)

In Razor Pages, dependencies are injected directly through the `PageModel` constructor:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Staff;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    // Inject DbContext via primary constructor or standard constructor
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Models.Staff> StaffMembers { get; set; } = [];
    public int ActiveCount { get; set; }

    public async Task OnGetAsync()
    {
        StaffMembers = await _context.Staff.ToListAsync();
        ActiveCount = StaffMembers.Count(s => s.IsActive == 1);
    }
}
```

---

## 6. Render Data in the Razor Page (`Pages/Staff/Index.cshtml`)

```cshtml
@page
@model MyRazorApp.Pages.Staff.IndexModel
@{
    ViewData["Title"] = "Staff Directory";
}

<h1>Staff Directory</h1>
<p>Total Active Employees: @Model.ActiveCount</p>

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
                <td>@member.Salary.ToString("C")</td>
                <td>
                    @if (member.IsActive == 1)
                    {
                        <span>Active</span>
                    }
                    else
                    {
                        <span>Inactive</span>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>

```

---

## Alternative Database Option

For connecting to Microsoft SQL Server instead of SQLite, see the [MSSQL Database Integration Guide](file:///Users/martincooper/Documents/github-demo-sites-for-modules/web-razor-pages/07-db-connection-mssql.md).

