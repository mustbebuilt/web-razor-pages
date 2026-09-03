# CRUD Architecture & Input Models in Razor Pages

## Overview: Managing Records with CRUD

**CRUD** is an acronym representing the four foundational operations of data management in web applications:
- **C**reate: Inserting new records into a database.
- **R**ead: Querying and displaying existing records (lists, summaries, and details).
- **U**pdate: Modifying existing database records.
- **D**elete: Removing records from the database.

In ASP.NET Core Razor Pages, implementing a robust CRUD subsystem requires organizing page handlers, routing, model validation, and user interface templates.

The **Staff CMS** (`Pages/CMS/`) in `MyRazorApp` demonstrates a complete administrative management area for staff records:

| Operation | Page / Endpoint | HTTP Method | Purpose |
| :--- | :--- | :--- | :--- |
| **Read (List)** | `/CMS/Index` | `GET` | View all staff records with status and action links |
| **Create** | `/CMS/Create` | `GET` & `POST` | Display an empty input form and insert a new staff member |
| **Update (Edit)** | `/CMS/Edit/{id:int}` | `GET` & `POST` | Load an existing staff record and save modified values |
| **Delete** | `/CMS/Delete/{id:int}` | `GET` & `POST` | Display a deletion confirmation prompt and safely remove the record |

---

## 1. Entity Models vs. Input Models

A common architecture mistake in web applications is binding HTML form submissions directly to Entity Framework database models (`Models/Staff.cs`). 

While this seems convenient for small demos, using dedicated **Input Models** (also known as *ViewModels* or *Data Transfer Objects*) is industry standard best practice.

```
       HTML Form Submission
                │
                ▼
      ┌──────────────────┐
      │    StaffInput    │  <-- Handles HTTP form binding, UI validation,
      │   (Input Model)  │      and data sanitization/trimming
      └─────────┬────────┘
                │ Mapping (.ToStaff() / .ApplyTo())
                ▼
      ┌──────────────────┐
      │   Models.Staff   │  <-- Clean EF Core database entity
      │  (Entity Model)  │      persisted to SQLite / SQL Server
      └──────────────────┘
```

### Why Use a Dedicated `StaffInput` Model?
1. **Security (Prevent Over-Posting / Mass Assignment)**: Direct binding to database entities allows malicious users to submit fields they shouldn't edit (such as primary key IDs, created timestamps, or permission flags).
2. **Data Formatting & Validation**: Input forms often require specific display names, custom regular expressions, or string-based date formats that don't match database column schemas.
3. **Data Normalization & Sanitization**: An Input Model allows you to trim extraneous whitespace (`FirstName.Trim()`) and transform data types before database persistence.

---

## 2. Implementing the Input Model (`Pages/CMS/StaffInput.cs`)

Here is the complete `StaffInput` class used across the CMS pages:

```csharp
using System.ComponentModel.DataAnnotations;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.CMS;

public class StaffInput
{
    // Parameterless constructor required by Razor Pages model binder during POST
    public StaffInput() { }

    // Constructor to populate input fields from an existing database entity
    public StaffInput(Models.Staff staff)
    {
        FirstName = staff.FirstName;
        LastName = staff.LastName;
        Email = staff.Email;
        Department = staff.Department;
        JobTitle = staff.JobTitle;
        HireDate = staff.HireDate;
        Salary = staff.Salary;
        IsActive = staff.IsActive;
    }

    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Hire date")]
    public string HireDate { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public double Salary { get; set; }

    [Display(Name = "Status")]
    public int IsActive { get; set; } = 1;

    // Convert input model to a new entity instance for Create
    public Models.Staff ToStaff() => new()
    {
        FirstName = FirstName.Trim(),
        LastName = LastName.Trim(),
        Email = Email.Trim(),
        Department = Department.Trim(),
        JobTitle = JobTitle.Trim(),
        HireDate = HireDate.Trim(),
        Salary = Salary,
        IsActive = IsActive == 1 ? 1 : 0
    };

    // Apply updated input model fields onto an existing tracked entity for Edit
    public void ApplyTo(Models.Staff staff)
    {
        staff.FirstName = FirstName.Trim();
        staff.LastName = LastName.Trim();
        staff.Email = Email.Trim();
        staff.Department = Department.Trim();
        staff.JobTitle = JobTitle.Trim();
        staff.HireDate = HireDate.Trim();
        staff.Salary = Salary;
        staff.IsActive = IsActive == 1 ? 1 : 0;
    }
}
```

---

## 3. Why is the Parameterless Constructor Necessary?

Notice that `StaffInput` defines two constructors:
```csharp
// 1. Parameterless constructor
public StaffInput() { }

// 2. Parameterized constructor
public StaffInput(Models.Staff staff)
{
    // ...
}
```

### Would `StaffInput` work without the parameterless constructor?

**No.** If you define the parameterized constructor `StaffInput(Models.Staff staff)` and omit the parameterless constructor `StaffInput() { }`, model binding on HTTP `POST` requests will break.

### How Model Binding and C# Constructors Interact:

1. **Default C# Constructor Rule**: In C#, if you do *not* write any constructor in a class, the compiler automatically generates a hidden default parameterless constructor for you.
2. **The Override**: The moment you add *any* constructor with parameters (such as `StaffInput(Models.Staff staff)` for the Edit page), the C# compiler **stops** providing the automatic parameterless constructor.
3. **Model Binder Instantiation**: During an HTTP `POST` submission (e.g. creating or editing a record), ASP.NET Core's Model Binder must instantiate a new `StaffInput` object before it can populate its properties from the incoming form fields. It relies on a public parameterless constructor (via reflection / `Activator.CreateInstance`) to do this.
4. **What happens if omitted?**: If the parameterless constructor is missing, the model binder cannot instantiate `StaffInput`, causing an `InvalidOperationException` at runtime:
   > *"Could not create an instance of type 'MyRazorApp.Pages.CMS.StaffInput'. Model bound complex types must have a parameterless constructor."*

> [!IMPORTANT]
> Whenever you add custom constructors to an Input Model or ViewModel in ASP.NET Core, always include an explicit parameterless constructor `public MyModel() { }` so the Model Binder can instantiate it during POST requests.

---

## 4. Data Annotations Explained

Data Annotations (`System.ComponentModel.DataAnnotations`) declare validation rules and UI metadata directly on C# properties:

### Validation Attributes
- **`[Required]`**: Ensures the user provides a non-empty, non-null value. If left blank, ASP.NET Core registers a validation error on `ModelState`.
- **`[EmailAddress]`**: Validates that the input follows standard email syntax (e.g. `user@domain.com`).
- **`[Range(min, max)]`**: Ensures numeric values fall within a specific threshold (e.g. salary must be $\ge 0$).
- **`[DataType(DataType.Date)]`**: Informs Razor Tag Helpers to generate an HTML5 `<input type="date" />` picker in supported browsers.

### Display Metadata
- **`[Display(Name = "First name")]`**: Specifies the user-friendly label rendered by `<label asp-for="FirstName"></label>`. Instead of displaying the raw C# identifier `FirstName`, the label will render as `First name`.

---

## 5. Entity Mapping Helpers

`StaffInput` includes two essential mapping methods to bridge the gap between user input and Entity Framework Core:

### A. Creation Mapping (`ToStaff()`)
Creates a fresh `Models.Staff` entity ready to be inserted via `_context.Staff.Add(...)`. String values are sanitized with `.Trim()` to remove accidental whitespace.

```csharp
public Models.Staff ToStaff() => new()
{
    FirstName = FirstName.Trim(),
    LastName = LastName.Trim(),
    Email = Email.Trim(),
    Department = Department.Trim(),
    JobTitle = JobTitle.Trim(),
    HireDate = HireDate.Trim(),
    Salary = Salary,
    IsActive = IsActive == 1 ? 1 : 0
};
```

### B. In-Place Update Mapping (`ApplyTo(...)`)
When editing an existing database record, EF Core tracks the loaded entity instance. Rather than replacing the object in memory, `ApplyTo(staff)` updates the tracked entity's properties in place:

```csharp
public void ApplyTo(Models.Staff staff)
{
    staff.FirstName = FirstName.Trim();
    staff.LastName = LastName.Trim();
    staff.Email = Email.Trim();
    staff.Department = Department.Trim();
    staff.JobTitle = JobTitle.Trim();
    staff.HireDate = HireDate.Trim();
    staff.Salary = Salary;
    staff.IsActive = IsActive == 1 ? 1 : 0;
}
```

> [!TIP]
> In the next tutorial, **`09b-crud-index-and-flash-messages.md`**, we will build the CMS management index page to display all staff members and handle status flash messages across page redirects.
