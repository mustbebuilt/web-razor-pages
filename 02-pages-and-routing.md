# Working with Razor Pages and PageModel Routing

## The PageModel and Handler Results

In ASP.NET Core Razor Pages, each page consists of a Razor view (`.cshtml`) paired with a `PageModel` code-behind file (`.cshtml.cs`).

Instead of routing requests through central controllers, Razor Pages uses **Handler Methods** inside the `PageModel` to process HTTP requests. Common handler method return types include `IActionResult`:

- `Page()`: Renders and returns the current Razor Page view.
- `RedirectToPage()`: Redirects the browser to another Razor Page.
- `NotFound()`: Returns a standard HTTP 404 response.
- `Content()`: Returns plain text or serialized content directly.

---

## Understanding Routing in Razor Pages

Routing in Razor Pages is convention-based and determined by the file hierarchy inside the `Pages/` directory.

`Program.cs` registers Razor Pages and maps incoming requests automatically:

```csharp
builder.Services.AddRazorPages();
// ...
app.MapRazorPages();
```

Incoming URLs match corresponding file paths under `Pages/`:

| Requested URL | Physical Razor Page File | PageModel Class | Executed Handler |
|---|---|---|---|
| `/` | `Pages/Index.cshtml` | `IndexModel` | `OnGet()` |
| `/Privacy` | `Pages/Privacy.cshtml` | `PrivacyModel` | `OnGet()` |
| `/News` | `Pages/News.cshtml` | `NewsModel` | `OnGet()` |
| `/Staff` | `Pages/Staff/Index.cshtml` | `Staff.IndexModel` | `OnGet()` |

---

## Adding a New Page (`News`)

To create a new route `/News`:

1. Create `Pages/News.cshtml.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages;

public class NewsModel : PageModel
{
    public void OnGet()
    {
        // Executes when user navigates to /News
    }
}
```

2. Create `Pages/News.cshtml`:

```cshtml
@page
@model NewsModel
@{
    ViewData["Title"] = "News";
}

<h1>Latest News</h1>
<p>Welcome to the news page.</p>
```

> [!IMPORTANT]
> The `@page` directive **must** be the very first line in a `.cshtml` file for ASP.NET Core to recognize it as a Razor Page.

---

## Creating Folder-Based Routes (`/Staff`)

As your app grows, grouping related pages into subdirectories under `Pages/` creates clean, hierarchical URLs.

For example, creating `Pages/Staff/Index.cshtml` matches the URL `/Staff`:

```cshtml
@page
@model MyRazorApp.Pages.Staff.IndexModel
@{
    ViewData["Title"] = "Staff Directory";
}

<h1>Staff Directory Page</h1>
```

And its code-behind `Pages/Staff/Index.cshtml.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.Staff;

public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}
```

---

## Why Choose Razor Pages for Page-Centric Apps?

1. **Co-location**: UI markup (`.cshtml`) and request handling logic (`.cshtml.cs`) live right next to each other.
2. **Simplified Routing**: No need to maintain route mapping tables or jump between separate controller and view folders.
3. **Clean Handler Operations**: Clear separation of GET and POST interactions per page (`OnGet()`, `OnPost()`).
