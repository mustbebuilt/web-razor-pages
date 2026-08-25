# Data Passing Techniques in ASP.NET Core Razor Pages (.NET 10)

In ASP.NET Core Razor Pages, there are four primary techniques for sharing data between server-side C# code and Razor view templates:

1. **Strongly-Typed `PageModel` Properties (Recommended)**
2. **`ViewData` Dictionary**
3. **`ViewBag` Dynamic Access**
4. **`TempData` / `[TempData]` Attribute**

---

## 1. Strongly-Typed `PageModel` Properties (Recommended)

Declaring public properties directly on the code-behind `PageModel` class provides compile-time safety, IDE IntelliSense support, and clean maintainability.

### PageModel (`Pages/DataToViews/ViewModel.cshtml.cs`)

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.DataToViews;

public class ViewModelModel : PageModel
{
    public string PageTitle { get; set; } = string.Empty;
    public List<StaffSummary> StaffMembers { get; set; } = [];

    public void OnGet()
    {
        PageTitle = "Staff Directory (Strongly-Typed PageModel Data)";
        StaffMembers =
        [
            new StaffSummary { Id = 1, Name = "Alice Johnson", Department = "Engineering", JobTitle = "Manager" },
            new StaffSummary { Id = 2, Name = "Brian Lee", Department = "Engineering", JobTitle = "Senior Developer" },
            new StaffSummary { Id = 3, Name = "Carla Gomez", Department = "Human Resources", JobTitle = "Specialist" }
        ];
    }
}

public class StaffSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
}
```

### View Template (`Pages/DataToViews/ViewModel.cshtml`)

```cshtml
@page
@model MyRazorApp.Pages.DataToViews.ViewModelModel
@{
    ViewData["Title"] = Model.PageTitle;
}

<h1>@Model.PageTitle</h1>

<ul>
@foreach (var staff in Model.StaffMembers)
{
    <li>@staff.Name - @staff.Department (@staff.JobTitle)</li>
}
</ul>
```

---

## 2. ViewData Dictionary

`ViewData` is a string-keyed dictionary available across layouts, pages, and partial views. It is ideal for passing simple layout values like page titles.

### Setting in PageModel:

```csharp
public void OnGet()
{
    ViewData["Message"] = "Hello from ViewData";
    ViewData["StaffCount"] = 3;
}
```

### Accessing in View:

```cshtml
<h1>@ViewData["Message"]</h1>

@{
    var count = ViewData["StaffCount"] as int? ?? 0;
}
<p>Total Staff: @count</p>
```

---

## 3. ViewBag Dynamic Property Access

`ViewBag` provides dynamic property syntax over the underlying `ViewData` dictionary within Razor views.

### Accessing in View:

```cshtml
@{
    ViewData["Title"] = "ViewBag Example";
}

<h1>@ViewBag.Title</h1>
```

---

## 4. TempData & POST-Redirect-GET

`TempData` persists values between HTTP requests. It is commonly used to store flash notification messages after form submissions.

### PageModel Handler (`Pages/DataToViews/TempDataDemo.cshtml.cs`):

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.DataToViews;

public class TempDataDemoModel : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnPostSetMessage()
    {
        StatusMessage = "Staff record updated successfully!";
        return RedirectToPage(); // POST-Redirect-GET pattern
    }
}
```

### View Rendering (`Pages/DataToViews/TempDataDemo.cshtml`):

```cshtml
@page
@model MyRazorApp.Pages.DataToViews.TempDataDemoModel

@if (!string.IsNullOrEmpty(Model.StatusMessage))
{
    <div class="alert alert-success">
        @Model.StatusMessage
    </div>
}

<form method="post" asp-page-handler="SetMessage">
    <button type="submit">Save Staff Record</button>
</form>
```

---

## Feature Comparison Matrix

| Feature | PageModel Properties | ViewData | ViewBag | TempData |
|---|---|---|---|---|
| **Strongly Typed** | ✅ Yes | ❌ No | ❌ No | ❌ No |
| **IntelliSense Support** | ✅ Full | ❌ No | ❌ No | ❌ No |
| **Compile-Time Checking** | ✅ Yes | ❌ No | ❌ No | ❌ No |
| **Survives Redirect** | ❌ No | ❌ No | ❌ No | ✅ Yes |
| **Primary Recommended Use** | Main Page Data & Models | Titles & Metadata | Dynamic View Prototyping | Flash / Status Messages |
