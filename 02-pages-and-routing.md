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

In web applications the concept of 'Routing' is the process by which the web server determines which code should be executed based on the URL requested by the user. It's the job of the routing system to map a URL to a specific piece of code that can generate a response.

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

> [!NOTE]
> Notice the use of `ViewData["Title"] = "News";` within the `@{ ... }` block. This is a dictionary that is used to pass data between the `PageModel` and the Razor view template. You will see this pattern used throughout the application to share data between `PageModel` classes and their corresponding Razor view templates. 

---

## HTTP Methods and OnGet / OnPost

A web browser communicates with a web server using HTTP methods. When a user navigates to a website, the browser sends an HTTP request to the web server. The web server then processes the request and sends an HTTP response back to the browser. The most common HTTP methods are GET, POST, PUT, DELETE, HEAD, and OPTIONS.

You can view the HTTP method in the browser's developer tools. For example, if you open the developer tools in Google Chrome, you can view the HTTP method by pressing F12. Then, click on the Network tab. You will see a list of all the HTTP requests that have been sent to the web server. You can then click on each request to view the details of the request, including the HTTP method.

Most web applications will require the ability to process more than one HTTP method. For example, a web application may need to process both GET and POST requests. In Razor Pages, this is done by creating handler methods for each HTTP method. The most common handler methods are `OnGet()`, `OnPost()`, and `OnDelete()`. Other handler methods include `OnPut()`, `OnHead()`, and `OnOptions()`. These methods are all part of the `IPageModel` interface. 

`OnGet()` is the most common handler method and is executed when a user navigates to a Razor Page using an HTTP GET request. `OnPost()` is executed when a user submits a form to a Razor Page using an HTTP POST request. A more detailed explanation of HTTP methods can be found on the W3Schools website [^1].

For example, `OnGet()` is used to display a form and `OnPost()` is used to process the form data.

You can think of the `OnGet()` method as the default method that is executed when a user navigates to a Razor Page. In effect, it's like a constructor for the page.

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

