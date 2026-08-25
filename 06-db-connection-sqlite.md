# Database Integration with SQLite and Entity Framework Core in Razor Pages

Now that we have seen how data can be passed between the different components of a web application using Model Binding and Razor expressions, it is time to look at storing and retrieving data from a database. 

We will use **SQLite** as our database for this guide. SQLite is a lightweight, zero-configuration, file-based database engine that is ideal for learning and rapid local development.

This guide demonstrates connecting an SQLite database (`Data/staff.db`) to an ASP.NET Core Razor Pages application. You can find the SQL script to create and seed the staff table in [resources/staff-sqlite.sql](resources/staff-sqlite.sql).

---

## 1. SQLite and Entity Framework Core

**Entity Framework Core (EF Core)** is an Object-Relational Mapper (ORM) that enables .NET developers to interact with databases using strongly-typed C# objects instead of writing raw SQL strings.

### What is an ORM?
An ORM acts as a translator between two different type systems: the object-oriented C# model representation and the relational database schema. EF Core automatically translates LINQ queries written in C# into SQL commands executed against the underlying database engine, returning query results back as C# objects.

---

## 2. Install SQLite EF Core NuGet Package

Open a terminal inside your project directory and add the EF Core SQLite provider package:

```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

---

## 3. Create the Staff Entity Model (`Models/Staff.cs`)

Create a model class in `Models/Staff.cs` mapping to your database table schema:

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
    public int IsActive { get; set; } // 0 = Inactive, 1 = Active
}
```

The above indicates that the staff table has the following columns:

* **StaffId**: The primary key of the table.
* **FirstName**: The first name of the staff member.
* **LastName**: The last name of the staff member.
* **Email**: The email address of the staff member.
* **Department**: The department of the staff member.
* **JobTitle**: The job title of the staff member.
* **HireDate**: The hire date of the staff member.
* **Salary**: The salary of the staff member.
* **IsActive**: Whether the staff member is active or not (1 = Active, 0 = Inactive).

The C# model class `Staff` is a strongly-typed representation of the `staff` table in the database.  The `[Key]` attribute indicates that the `StaffId` property is the primary key of the table.  The `[Column("column_name")]` attribute indicates that the property is mapped to a specific column in the database table.  Although not used in this example, other common attributes include:

* `[Required]`: indicates that the property is required and cannot be null.
* `[MaxLength(n)]`: indicates that the property has a maximum length of n characters.
* `[EmailAddress]`: indicates that the property is an email address.

---

## 4. Create the Database Context (`Data/ApplicationDbContext.cs`)

A `DbContext` represents a session with the database and acts as the central manager for data operations. It tracks entity states, maps C# classes to database tables, and executes queries.

Create `Data/ApplicationDbContext.cs` inheriting from `DbContext`:

```csharp
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<Staff> Staff { get; set; } = null!;
}
```

### Key Components Explained:
- **`DbContextOptions<ApplicationDbContext>`**: A strongly-typed configuration object containing settings such as the database provider and connection string. Passing this to `: base(options)` allows ASP.NET Core's Dependency Injection system to configure the database context at application startup.
- **`DbSet<Staff> Staff`**: Represents the `staff` database table. LINQ queries executed against this property return `Staff` entity instances.

---

## 5. Configure Connection String and Register Service

In `appsettings.json`, add your SQLite connection string pointing to your database file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/staff.db"
  }
}
```

Register `ApplicationDbContext` in `Program.cs` before `builder.Build()`:

```csharp
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Register SQLite DbContext service with Dependency Injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
```

---

## 6. Inject DbContext into the PageModel (`Pages/Staff/Index.cshtml.cs`)

In Razor Pages, dependencies are injected directly into the `PageModel` constructor:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.Staff;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    // Dependency Injection via constructor
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

### Code Explanation:
- **Dependency Injection**: ASP.NET Core automatically instantiates and passes the configured `ApplicationDbContext` to the `IndexModel` constructor when a request for the page arrives.
- **`async` / `await`**: The `OnGetAsync()` method executes asynchronously, allowing the server thread to handle other web requests while the database reads from disk.
- **`ToListAsync()`**: Asynchronously queries the `Staff` table and returns all records as a C# list.
- **`ActiveCount`**: Uses LINQ to filter and count active staff members (`IsActive == 1`).

---

## 7. Render Data in the Razor Page (`Pages/Staff/Index.cshtml`)

Display the queried data in the Razor view template using HTML and Razor expressions:

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

For connecting to Microsoft SQL Server instead of SQLite, see the [MSSQL Database Integration Guide](07-db-connection-mssql.md).
