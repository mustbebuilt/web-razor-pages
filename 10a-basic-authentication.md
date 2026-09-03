# Basic Session-Based CMS Authentication

## Overview: Protecting the CMS Area

When building web applications, administrative areas—such as our **Staff CMS** (`Pages/CMS/`)—must be protected from unauthorized access.

ASP.NET Core provides multiple authentication strategies. For simple internal tools or single-administrator prototypes, a **Session-Based Password Authentication** system provides a lightweight gatekeeper without requiring complex user tables or Identity scaffolding.

```
 Unauthenticated User visits /CMS/Index
                   │
                   ▼
    ┌──────────────────────────────┐
    │  CmsPageModel (Page Filter)  │  <-- Intercepts request before handler executes
    └──────────────┬───────────────┘
                   │ Not Authenticated
                   ▼
       Redirect to /CMS/Login
       (?returnUrl=/CMS/Index)
                   │
                   ▼
       User submits admin password
                   │
         Valid? ───┴───► Invalid? ──► Display error message
           │
           ▼
    ┌──────────────────────────────┐
    │  CmsAuthentication.SignIn()  │  <-- Sets Session["CmsAuthenticated"] = "true"
    └──────────────┬───────────────┘
                   │
                   ▼
       Redirect back to returnUrl
```

This guide explains how session-based authentication was implemented in `MyRazorApp/Pages/CMS/`.

---

## 1. Configuring Password & Session Services (`Program.cs` & `appsettings.Development.json`)

### A. Defining the Admin Password in Configuration
In `appsettings.Development.json`, add a `Security` configuration section:

```json
{
  "DetailedErrors": true,
  "Security": {
    "AdminPassword": "test"
  }
}
```

> [!TIP]
> In production environments, sensitive secrets like `AdminPassword` should be supplied via environment variables (e.g., `Security__AdminPassword`) or Azure Key Vault rather than committed to source control.

### B. Registering Services in `Program.cs`
In `Program.cs`, register the `CmsAuthentication` service as a singleton and configure secure session cookie settings:

```csharp
using MyRazorApp.Pages.CMS;

// Register custom CMS Authentication helper
builder.Services.AddSingleton<CmsAuthentication>();

// Enable Session State with strict security settings
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// ...
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseSession(); // MUST be called after UseRouting() and before MapRazorPages()

app.MapStaticAssets();
app.MapRazorPages();
```

---

## 2. The Authentication Helper (`Pages/CMS/CmsAuthentication.cs`)

The `CmsAuthentication` class encapsulates session management and password validation:

```csharp
using System.Security.Cryptography;
using System.Text;

namespace MyRazorApp.Pages.CMS;

public sealed class CmsAuthentication
{
    private const string SessionKey = "CmsAuthenticated";
    private readonly string? _adminPassword;

    public CmsAuthentication(IConfiguration configuration)
    {
        _adminPassword = configuration["Security:AdminPassword"];
    }

    public bool IsConfigured => !string.IsNullOrEmpty(_adminPassword);

    public bool IsAuthenticated(HttpContext httpContext) =>
        httpContext.Session.GetString(SessionKey) == "true";

    public bool IsValidPassword(string password)
    {
        if (!IsConfigured)
        {
            return false;
        }

        var configuredBytes = Encoding.UTF8.GetBytes(_adminPassword!);
        var suppliedBytes = Encoding.UTF8.GetBytes(password);

        // Constant-time comparison to prevent timing attack vulnerabilities
        return CryptographicOperations.FixedTimeEquals(configuredBytes, suppliedBytes);
    }

    public void SignIn(HttpContext httpContext) =>
        httpContext.Session.SetString(SessionKey, "true");

    public void SignOut(HttpContext httpContext) =>
        httpContext.Session.Remove(SessionKey);
}
```

### Why Use `CryptographicOperations.FixedTimeEquals`?
Standard string equality (`password == _adminPassword`) returns `false` as soon as the first mismatched character is found. Attackers can measure response times down to nanoseconds to guess characters one by one (a **timing attack**). `FixedTimeEquals` takes the exact same amount of time regardless of where mismatches occur, preventing timing leakage.

---

## 3. Protecting CMS Pages with a Base `CmsPageModel`

Rather than copying authentication checks into every `OnGet()` and `OnPost()` method across all CMS pages, we create an abstract base class `CmsPageModel` that overrides `OnPageHandlerExecutionAsync`:

```csharp
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.CMS;

public abstract class CmsPageModel : PageModel
{
    private readonly CmsAuthentication _authentication;

    protected CmsPageModel(CmsAuthentication authentication)
    {
        _authentication = authentication;
    }

    public override async Task OnPageHandlerExecutionAsync(
        PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next)
    {
        if (!_authentication.IsAuthenticated(HttpContext))
        {
            // Redirect unauthenticated requests to the Login page with returnUrl preserved
            context.Result = RedirectToPage("/CMS/Login", new
            {
                returnUrl = Request.Path + Request.QueryString
            });
            return;
        }

        await next();
    }
}
```

### Inheriting `CmsPageModel` Across CMS Pages
All CMS pages (`IndexModel`, `CreateModel`, `EditModel`, `DeleteModel`, `LogoutModel`) inherit from `CmsPageModel` instead of `PageModel`:

```csharp
public class IndexModel : CmsPageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context, CmsAuthentication authentication) 
        : base(authentication)
    {
        _context = context;
    }
    // ...
}
```

Any attempt to access `/CMS/Index`, `/CMS/Create`, `/CMS/Edit/1`, or `/CMS/Delete/1` while unauthenticated is automatically intercepted and redirected to `/CMS/Login`.

---

## 4. The CMS Login Page (`Pages/CMS/Login.cshtml` & `.cs`)

### View Implementation (`Pages/CMS/Login.cshtml`)
```cshtml
@page
@model MyRazorApp.Pages.CMS.LoginModel
@{
    ViewData["Title"] = "CMS Login";
}

<h1>CMS Login</h1>
<p>Enter the administrator password to manage staff records.</p>

<form method="post">
    <input type="hidden" asp-for="ReturnUrl" />
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
    <div class="form-field">
        <label asp-for="Password" class="form-label"></label>
        <input asp-for="Password" type="password" class="search-input full-width" autocomplete="current-password"
               autofocus />
        <span asp-validation-for="Password" class="text-danger validation-error"></span>
    </div>
    <button type="submit" class="btn btn-primary">Sign in</button>
</form>
```

### PageModel Implementation (`Pages/CMS/Login.cshtml.cs`)
```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.CMS;

public class LoginModel : PageModel
{
    private readonly CmsAuthentication _authentication;

    public LoginModel(CmsAuthentication authentication)
    {
        _authentication = authentication;
    }

    [BindProperty]
    [Required]
    [Display(Name = "Admin password")]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!_authentication.IsValidPassword(Password))
        {
            ModelState.AddModelError(string.Empty, "The password is incorrect or has not been configured.");
            return Page();
        }

        // Set session variable
        _authentication.SignIn(HttpContext);

        // Prevent Open Redirect attacks by validating ReturnUrl
        return Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl!)
            : RedirectToPage("/CMS/Index");
    }
}
```

> [!SECURITY]
> **Open Redirect Prevention**: Always validate return URLs with `Url.IsLocalUrl(ReturnUrl)`. An attacker could craft a link like `/CMS/Login?returnUrl=https://evil.com` to trick authenticated users into being redirected to a phishing site.

---

## 5. The CMS Sign Out Flow (`Pages/CMS/Logout.cshtml` & `.cs`)

Signing out removes the session key and redirects back to the login screen:

```csharp
namespace MyRazorApp.Pages.CMS;

public class LogoutModel : CmsPageModel
{
    private readonly CmsAuthentication _authentication;

    public LogoutModel(CmsAuthentication authentication) : base(authentication)
    {
        _authentication = authentication;
    }

    public IActionResult OnPost()
    {
        _authentication.SignOut(HttpContext);
        return RedirectToPage("/CMS/Login");
    }
}
```

In `Pages/CMS/Index.cshtml`, a sign-out button is rendered alongside the "Add staff member" action:

```cshtml
<p>
    <a asp-page="Create" class="btn btn-primary">Add staff member</a>
    <form method="post" asp-page="Logout" style="display: inline;">
        <button type="submit" class="btn btn-secondary">Sign out</button>
    </form>
</p>
```

---

## 6. Limitations & Next Steps

While this basic session authentication is easy to set up, it has several architectural limitations:

| Limitation | Impact | Solution |
| :--- | :--- | :--- |
| **Shared Password** | All administrators share one password; no individual accountability or audit log. | Individual user accounts stored in the database. |
| **Plain Text Config** | Passwords stored in JSON configuration files risk accidental exposure. | Store salted, cryptographic password hashes in the database. |
| **No Roles / Permissions** | All authenticated users have full access to create, edit, and delete. | Role-Based Access Control (RBAC) via Claims. |
| **In-Memory Sessions** | Server restarts or load-balanced multiple servers invalidate user sessions. | Persistent encrypted Cookie Authentication (`HttpContext.SignInAsync`). |

> [!TIP]
> In **`10-custom-db-authentication.md`**, we explore how to upgrade this basic security model into a full database-backed authentication system with hashed passwords and cookie claims.
