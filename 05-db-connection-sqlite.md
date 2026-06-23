# Working with Databases (SQLite)

## SQLite

We have a simple database table of staff members located at `/data/staff.db` that was created with:

```SQL
CREATE TABLE "staff" (
	"staff_id"	INTEGER,
	"first_name"	TEXT NOT NULL,
	"last_name"	TEXT NOT NULL,
	"email"	TEXT NOT NULL UNIQUE,
	"department"	TEXT NOT NULL DEFAULT 'General',
	"job_title"	TEXT NOT NULL,
	"hire_date"	TEXT NOT NULL DEFAULT (DATE('now')),
	"salary"	REAL NOT NULL,
	"is_active"	INTEGER NOT NULL DEFAULT 1 CHECK("is_active" IN (0, 1)),
	PRIMARY KEY("staff_id" AUTOINCREMENT)
)
```

We can connect to a whole range of database types but we start with a simple SQLite example.

## Entity Framework

Entity Framework Core (EF Core) is an Object-Relational Mapper (ORM) that enables .NET developers to work with a database using `C#` objects.

To list your staff members into a new Razor View, you need to connect your database to your ASP.NET Core MVC application and it is Entity Framework that will help us here.

## Install the Entity Framework SQLite NuGet Package

Open your terminal inside your project directory and install the EF Core SQLite provider:

```terminal
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.9
```

## Create the Staff Entity Model

Create a class inside your Models folder that matches your database schema exactly.

```csharp
// File: Models/Staff.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models;

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
    public int IsActive { get; set; } // 0 or 1 matching SQLite integer constraint
}
```

> [!Note]
> We have stuck to standard naming conventions here.
> For SQLite we have used snake_case (Lowercase, Underscores) such as `first_name`.
> For the `C#` model we used PascalCase (Capitalised)  such as `FirstName`.
> These obviously don't match but notice the use of the  [Column("field_name")] attributes in `Staff.cs`.  These are Data Annotations which allow the naming conventions to translate between SQLite and the model.

## Create the Database Context (DbContext)

Part of the Entity Framework Core, the `DbContext` class represents a session with a database.

The `DbContext` class has methods for Adding, Modifying and Deleting data. For extracting (querying) data, use the `DbSet` property of `DbContext`.

Create a context class to manage database connections and query operations.

```csharp
using MyMvcApp.Models;
namespace MyMvcApp.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Staff> Staff { get; set; } = null!;
}
```
Part of the Entity Framework Core, the `DbSet` class represents a collection for a given entity.

## Configure the Connection String and Register the Service
Open your `appsettings.json` file and add your SQLite file path.

> [!Note]
> If `/data/staff.db` is an absolute system path, use `Data Source=/data/staff.db`;. If it is relative to your project folder, use `Data Source=data/staff.db;`.

```JSON
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=data/staff.db;"
  }
}
```

Next, open `Program.cs` to register the database context service before `builder.Build()`:

```csharp
using Microsoft.EntityFrameworkCore;using MyMvcApp.Data;
var builder = WebApplication.CreateBuilder(args);
// Register SQLite Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## Build the Staff Controller
Edit the `StaffController.cs` created earlier to pull the staff records from the database and send them down to your presentation layer.

```csharp
using Microsoft.AspNetCore.Mvc;using MyMvcApp.Data;
namespace MyMvcApp.Controllers;
public class StaffController : Controller
{
    private readonly ApplicationDbContext _context;

    // Inject database context through the constructor
    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Pull all staff records from SQLite database file
        var staffList = _context.Staff.ToList();
        return View(staffList);
    }
}
```

## Create the Razor View Layout
Edit the existing view `Views/Staff/Index.cshtml` to render the data in a `HTML` table.

@* File: Views/Staff/Index.cshtml *@
```csharp
@model IEnumerable<MyMvcApp.Models.Staff>

@{
    ViewData["Title"] = "Staff Directory";
}

<div class="container mt-4">
    <h1>Staff Directory</h1>
    <p class="text-muted">Total Active Employees: @Model.Count(s => s.IsActive == 1)</p>

    <table class="table table-striped table-hover mt-3">
        <thead class="table-dark">
            <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Department</th>
                <th>Job Title</th>
                <th>Salary</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var member in Model)
            {
                <tr>
                    <td>@member.FirstName @member.LastName</td>
                    <td>@member.Email</td>
                    <td>@member.Department</td>
                    <td>@member.JobTitle</td>
                    <td>@member.Salary.ToString("C")</td>
                    <td>
                        @if (member.IsActive == 1)
                        {
                            <span class="badge bg-success">Active</span>
                        }
                        else
                        {
                            <span class="badge bg-danger">Inactive</span>
                        }
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

We now have a data driven page with the staff details displayed.




