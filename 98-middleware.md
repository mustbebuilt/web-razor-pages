# Razor Pages File Structure and Middleware Pipeline

### 1. Main Directory Structure

The project structure in ASP.NET Core Razor Pages is page-focused:

#### `Pages/`
Contains all application pages. Each page consists of a pair of files:
- `.cshtml`: Razor HTML markup template.
- `.cshtml.cs`: C# `PageModel` code-behind handling page logic and HTTP requests.

Special files inside `Pages/`:
- `_ViewStart.cshtml`: Sets default layout page (`_Layout.cshtml`).
- `_ViewImports.cshtml`: Centralizes global namespaces and Razor Tag Helpers.
- `Shared/`: Holds master layouts (`_Layout.cshtml`), error pages, and shared partial views.

#### `Models/` & `Data/`
Holds domain entity classes (e.g. `Staff.cs`) and Entity Framework Core database context (`ApplicationDbContext.cs`).

#### `wwwroot/`
Contains static web assets served directly to the browser (CSS stylesheets, JavaScript scripts, images, icons).

---

### 2. Key Startup Files

- `Program.cs`: Configures application dependency injection services and the HTTP request middleware pipeline.
- `appsettings.json`: Stores configuration settings (connection strings, logging levels, environment settings).
- `Pages/_ViewStart.cshtml`: Configures shared layout templates across all pages.
- `Pages/_ViewImports.cshtml`: Enables Razor Tag Helpers (`@addTagHelper`) and default namespaces globally.

---

### 3. Middleware and Request Pipeline (`Program.cs`)

`Program.cs` configures the middleware pipeline in order:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register Razor Pages services in Dependency Injection
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages(); // Maps incoming URL paths to corresponding Razor Pages under Pages/

app.Run();
```

---

### 4. How Request Processing Flows

1. An incoming browser HTTP request (e.g. `GET /Staff`) enters the middleware pipeline.
2. `UseRouting()` matches the request path `/Staff` to the Razor Page `Pages/Staff/Index.cshtml`.
3. The framework instantiates `Pages.Staff.IndexModel` and injects required dependencies (such as `ApplicationDbContext`).
4. The handler method `OnGet()` (or `OnGetAsync()`) executes to prepare data properties.
5. The Razor template `Pages/Staff/Index.cshtml` renders into HTML inside `Pages/Shared/_Layout.cshtml` and returns an HTTP response.
