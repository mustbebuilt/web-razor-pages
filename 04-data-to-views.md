# Passing Data in Razor Pages

## Data Passing Mechanisms in Razor Pages

In ASP.NET Core Razor Pages, data flows between server-side logic (`PageModel`) and client-side presentation (`.cshtml` view). 

The primary ways to pass data include:

1. **Strongly-Typed PageModel Properties (Recommended)**
2. **`ViewData` Dictionary**
3. **`TempData` Dictionary / `[TempData]` Attribute**

---

## 1. Strongly-Typed PageModel Properties (Recommended)

In Razor Pages, the `PageModel` class acts as its own ViewModel. Declaring public properties on the `PageModel` makes them accessible in the view via `@Model`.

### Step 1: Define Properties in `PageModel`

```csharp
// File: Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
    public string Message { get; set; } = string.Empty;
    public List<string> FeaturedStaff { get; set; } = [];

    public void OnGet()
    {
        Message = "Welcome to ASP.NET Core Razor Pages";
        FeaturedStaff = ["Alice Johnson", "Brian Lee", "Carla Gomez"];
    }
}
```

### Step 2: Render Properties in the View (`Index.cshtml`)

```cshtml
@page
@model IndexModel
@{
    ViewData["Title"] = "Home Page";
}

<h1>@Model.Message</h1>

@if (Model.FeaturedStaff.Count == 0)
{
    <p>No staff members available.</p>
}
else
{
    <p>Available staff members count: @Model.FeaturedStaff.Count</p>
    <ul>
        @foreach (var staff in Model.FeaturedStaff)
        {
            <li>@staff</li>
        }
    </ul>
}
```

---

## 2. Dynamic UI Controls (Dropdown `<select>`)

Properties from the `PageModel` can easily populate interactive HTML form controls:

```cshtml
<label for="staffSelect">Choose a staff member:</label>
<select id="staffSelect" class="form-select">
    @foreach (var staff in Model.FeaturedStaff)
    {
        <option value="@staff">@staff</option>
    }
</select>
```

---

## 3. ViewData for Small Metadata

`ViewData` is a weakly-typed dictionary suitable for small data values like page titles:

```csharp
// In PageModel handler:
ViewData["Title"] = "Staff Directory";
```

```cshtml
<!-- In _Layout.cshtml or view template: -->
<title>@ViewData["Title"]</title>
```

---

## 4. TempData for Flash Messages across Redirects

`TempData` persists data across a single HTTP redirect, making it ideal for POST-Redirect-GET patterns:

```csharp
// File: Pages/DataToViews/TempDataDemo.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.DataToViews;

public class TempDataDemoModel : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnPostSave()
    {
        StatusMessage = "Staff record updated successfully!";
        return RedirectToPage();
    }
}
```

```cshtml
@page
@model TempDataDemoModel

@if (!string.IsNullOrEmpty(Model.StatusMessage))
{
    <div class="alert alert-success">
        @Model.StatusMessage
    </div>
}

<form method="post" asp-page-handler="Save">
    <button type="submit">Save Staff Record</button>
</form>
```

---

## Summary Comparison

| Technique | Type Safety | Scope | Primary Use Case |
|---|---|---|---|
| **PageModel Property** | ✅ Strong (Compile-time) | Current Request | Main view data & form bindings |
| **ViewData** | ❌ Weak (Dictionary lookup) | Current Request | Page titles & small layout values |
| **TempData** | ❌ Weak (Dictionary / Attribute) | Across Redirect | Status messages & flash alerts |
