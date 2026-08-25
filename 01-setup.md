# MyRazorApp Setup & Quick Start

This quick start guide sets up a web application using the ASP.NET Core **Razor Pages** template. The aim of this guide is to:

- Create and configure a basic Razor Pages application (`MyRazorApp`).
- Review the structure of a Razor Pages application (`Pages/`, `wwwroot/`, `Program.cs`).
- Customize the Layout template and UI structure.
- Add static files to the `wwwroot` folder.
- Add dynamic Razor Pages with C# code-behind `PageModel` classes.
- Pass data between `PageModel` handlers and Razor view templates.

## Server-Side Technologies

Razor pages is one of many server-side technologies that can be used to create web applications. These technologies generate HTML and send it to the browser, often refered to as dynamically generated web pages. Other server-side technologies include PHP, Java Server Pages (JSP), and Ruby on Rails. Indeed .net offers other server-side technologies such as MVC and Blazor. 

Two of the key benefits of a server-side approach are:

- Easy templating for page maintenance and consistent styling across pages.
- The use of a backend database to build dynamic data-driven web applications.

Most web applications will reach a point where client-side technologies such as Javascript and CSS are insufficient to meet the requirements of the application. This is particularly true for applications that require a backend database to store and retrieve data. In such cases, a server-side approach is required. In this tutorial we will use .net and the Razor Pages framework to build a web application that uses a backend database to store and retrieve data.

---

## Requirements

This tutorial requires:

- **Visual Studio Code** - Free, cross-platform ([download](https://code.visualstudio.com/))
- **.NET SDK 10.0+** - Free, cross-platform ([download](https://dotnet.microsoft.com/en-us/download))
- Recommended extensions: C# Dev Kit, .NET Install Tool.

---

## Project Creation and First Run

### Step 1. Create the Application

To create a starter Razor Pages project in Visual Studio Code, open your terminal and run:

```bash
dotnet new razor -n MyRazorApp
```

### Step 2. Move into the Project Folder

If using VS Code, reopen the folder in the terminal so the path is set to `MyRazorApp`.

Alternatively, move into the project folder using the terminal:

```bash
cd MyRazorApp
```

### Step 3. Build and Run

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

Each Razor Page consists of two paired files:
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

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
```

URLs map directly to file paths inside the `Pages` directory (e.g. `/Privacy` maps to `Pages/Privacy.cshtml`).

We will explore these concepts further as we build dynamic pages and bind data models.