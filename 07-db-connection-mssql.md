# Working with Databases (Microsoft SQL Server / MSSQL)

## Overview: SQL Server and Entity Framework Core

While SQLite is lightweight and file-based (making it great for rapid local prototyping), production enterprise web applications often connect to **Microsoft SQL Server (MSSQL)** or **Azure SQL Database**.

Entity Framework Core (EF Core) allows you to target SQL Server with minimal code changes beyond changing the provider package and connection string. You can find the SQL script to create and seed the staff table in [resources/staff-mssql.sql](resources/staff-mssql.sql).

---

## 1. Install the EF Core SQL Server NuGet Package

Open your terminal inside your project directory and install the SQL Server provider package:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

If you plan to run EF Core database migrations, also install the design tools package:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

## 2. Create the Entity Model (`Models/Staff.cs`)

Define your C# entity class representing the SQL Server `Staff` table:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models;

[Table("Staff")]
public class Staff
{
    [Key]
    [Column("StaffId")]
    public int StaffId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("LastName")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("Department")]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("JobTitle")]
    public string JobTitle { get; set; } = string.Empty;

    [Column("HireDate")]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Column("Salary", TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; } = true;
}
```

---

## 3. Create the Database Context (`Data/ApplicationDbContext.cs`)

The `DbContext` handles connections and translates LINQ queries into SQL Server T-SQL statements:

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

## 4. Configure Connection Strings in `appsettings.json`

Add your SQL Server connection string under `ConnectionStrings` in `appsettings.json`.

### Option A: LocalDB (Windows default development server)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StaffDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Option B: Local SQL Server / Docker Container (SQL Authentication)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=StaffDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
  }
}
```

### Option C: Azure SQL Database
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=StaffDb;Persist Security Info=False;User ID=youradmin;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

---

## 5. Register SQL Server Provider in `Program.cs`

In `Program.cs`, replace `UseSqlite()` with `UseSqlServer()`:

```csharp
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Register SQL Server DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
```

---

## 6. Run EF Core Migrations (Creating the SQL Server Database)

Unlike SQLite where single file databases are created on demand, SQL Server uses EF Core Migrations to generate and maintain database schemas:

```bash
# 1. Create a migration snapshot
dotnet ef migrations add InitialCreate

# 2. Apply the migration to create the SQL Server database & tables
dotnet ef database update
```

---

## 7. Inject DbContext into the Razor PageModel (`Pages/Staff/Index.cshtml.cs`)

Dependencies are injected into the `PageModel` constructor via ASP.NET Core Dependency Injection:

```csharp
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

    public async Task OnGetAsync()
    {
        // Asynchronously query SQL Server database
        StaffMembers = await _context.Staff
            .AsNoTracking()
            .OrderBy(s => s.LastName)
            .ToListAsync();

        ActiveCount = StaffMembers.Count(s => s.IsActive);
    }
}
```

---

## 8. Render Data in the Razor Page (`Pages/Staff/Index.cshtml`)

```cshtml
@page
@model MyRazorApp.Pages.Staff.IndexModel
@{
    ViewData["Title"] = "Staff Directory (MSSQL)";
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
                    @if (member.IsActive)
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

## SQLite vs. Microsoft SQL Server Comparison

| Feature | SQLite | Microsoft SQL Server (MSSQL) |
|---|---|---|
| **Storage Model** | Single local `.db` file | Server process / Docker container / Cloud service |
| **NuGet Package** | `Microsoft.EntityFrameworkCore.Sqlite` | `Microsoft.EntityFrameworkCore.SqlServer` |
| **Connection String** | `"Data Source=data/staff.db;"` | `"Server=localhost;Database=StaffDb;..."` |
| **Data Types** | Flexible (integer 0/1 for booleans) | Strict native types (`bit`, `decimal(18,2)`, `datetime2`) |
| **Schema Management** | File creation or migrations | EF Core Migrations (`dotnet ef database update`) |
| **Best Used For** | Prototyping, testing, mobile/desktop apps | Production web applications, enterprise systems, cloud |
