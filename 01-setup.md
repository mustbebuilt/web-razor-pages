# MyRazorApp Quick Start & Setup

This quick start guide sets up a web application using the ASP.NET Core **Razor Pages** template. The aim of this guide is to:

- Create and configure a basic Razor Pages application.
- Review the file structure of a Razor Pages application (`Pages/`, `wwwroot/`, `Program.cs`).
- Edit the Layout template and customize HTML structure and CSS styling.
- Add static files to `wwwroot`.
- Add dynamic Razor Pages with C# code-behind `PageModel` classes.
- Pass data between `PageModel` handlers and Razor view templates.

---

## Requirements

This tutorial requires:

- **Visual Studio Code** - Free, cross-platform ([download](https://code.visualstudio.com/))
- **.NET SDK 10.0+** - Free, cross-platform ([download](https://dotnet.microsoft.com/en-us/download))
- Recommended extensions: C# Dev Kit, .NET Install Tool.

---

## Project Creation and First Run

### Create the Application

To create a starter Razor Pages project in Visual Studio Code, open your terminal and run:

```bash
dotnet new razor -n MyRazorApp
```
*(Alternatively, `dotnet new webapp -n MyRazorApp` creates the same Razor Pages project structure).*

### Move into the Project Folder

```bash
cd MyRazorApp
```

### Build and Run

```bash
dotnet build
dotnet run
```

At this point, the app runs the default ASP.NET Core Razor Pages template.

> [!NOTE]
> The app runs on `localhost` for development.
> To enable automatic UI and code reloading without restarting the web server, run:
> ```bash
> dotnet watch run
> ```

---

## What Has Just Happened?

The default application consists of standard pages such as the Home (`Index`) page and Privacy page. Let's look at how these pages work in Razor Pages.

### Razor Pages & PageModels

Navigate to the `Pages` directory in the application. Notice files like `Index.cshtml` and `Privacy.cshtml`. 

Each Razor Page typically consists of two paired files:
1. **`.cshtml` file**: The presentation view template combining HTML and Razor syntax.
2. **`.cshtml.cs` file**: The C# code-behind `PageModel` class handling HTTP requests (`OnGet()`, `OnPost()`) and holding page properties.

Example `@` Razor syntax transition:

```cshtml
@{
    ViewData["Title"] = "Home Page";
}
```

Razor transitions seamlessly between HTML markup and C# code using the `@` character.

### Shared Layout Files

The markup in `Index.cshtml` and `Privacy.cshtml` is concise because both pages inherit a shared layout template located at `Pages/Shared/_Layout.cshtml`. This provides standard document structure (`<!DOCTYPE html>`, `<html>`, `<head>`, `<body>`, navigation header, and footer).

### Static Files (`wwwroot`)

Static client-side assets such as CSS stylesheets, JavaScript files, and images are stored in the `wwwroot` directory and served directly by the web server.

### Startup Pipeline (`Program.cs`)

In Razor Pages, request handling and page routing are configured in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register Razor Pages services
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages(); // Maps incoming URLs to matching Razor Pages under the Pages/ directory

app.Run();
```

URLs map directly to file paths inside the `Pages` directory (e.g. `/Privacy` maps to `Pages/Privacy.cshtml`).

We will explore these concepts further as we build dynamic pages and bind data models.