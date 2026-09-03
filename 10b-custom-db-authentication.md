# Upgrading to Database Authentication with Hashed Passwords

## Overview: From Basic Session to Individual Staff Authentication

In [10-basic-authentication.md](10-basic-authentication.md), we implemented a lightweight gatekeeper for the CMS using a single shared administrator password stored in configuration and verified via session state.

While effective for early prototypes, real-world applications require **individual user accounts** with distinct credentials, encrypted password storage, and role-based permissions.

This guide explains how to upgrade the basic session security by:
1. **Extending the Staff Entity & Database**: Adding `PasswordHash` and `Role` columns to the `staff` table.
2. **Cryptographic Password Hashing**: Using ASP.NET Core's built-in `PasswordHasher<T>` (PBKDF2 / HMAC-SHA256 with salted hashes).
3. **Cookie Claims Authentication**: Replacing in-memory session flags with encrypted, standard ASP.NET Core Cookie Authentication (`HttpContext.SignInAsync`).
4. **Role-Based Authorization**: Restricting sensitive operations (such as deleting staff members) to users with the `Admin` role.

---

## 1. Upgrading the Database Schema & Entity Model

### A. Updating the SQL Schema
Add `password_hash` and `role` columns to the `staff` table:

```sql
-- SQLite Schema Migration
ALTER TABLE staff ADD COLUMN password_hash TEXT;
ALTER TABLE staff ADD COLUMN role TEXT NOT NULL DEFAULT 'Staff';
```

### B. Updating the Staff Entity (`Models/Staff.cs`)
Add the corresponding properties to `Models/Staff.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models;

[Table("staff")]
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
    public int IsActive { get; set; } = 1;

    // Secure, salted password hash (never store plain text!)
    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    // Role-based permissions (e.g. "Admin", "Manager", "Staff")
    [Column("role")]
    public string Role { get; set; } = "Staff";
}
```

---

## 2. Implementing Secure Password Hashing

ASP.NET Core provides `PasswordHasher<TUser>` in `Microsoft.AspNetCore.Identity`, which implements industry standard PBKDF2 hashing with a cryptographically secure, per-user 128-bit random salt and automatic work factor iterations.

Create a dedicated password hashing service (`Services/PasswordHasherService.cs`):

```csharp
using Microsoft.AspNetCore.Identity;
using MyRazorApp.Models;

namespace MyRazorApp.Services;

public interface IPasswordHasherService
{
    string HashPassword(Staff staff, string password);
    bool VerifyPassword(Staff staff, string hashedPassword, string providedPassword);
}

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<Staff> _hasher = new();

    public string HashPassword(Staff staff, string password)
    {
        return _hasher.HashPassword(staff, password);
    }

    public bool VerifyPassword(Staff staff, string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(staff, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
```

> [!IMPORTANT]
> **Why Salted Hashes Matter**: Storing plain text or simple MD5/SHA-256 hashes is insecure because identical passwords produce identical hashes (making them vulnerable to Rainbow Table lookups). `PasswordHasher<T>` generates a unique random salt for every password so that even two identical passwords produce completely different hash strings.

---

## 3. Configuring Cookie Authentication in `Program.cs`

Replace the custom session configuration with standard ASP.NET Core Cookie Authentication:

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using MyRazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Password Hasher service
builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

// 2. Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/CMS/Login";
        options.LogoutPath = "/CMS/Logout";
        options.AccessDeniedPath = "/CMS/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// 3. Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
```

---

## 4. Individual Staff Login Page (`Pages/CMS/Login.cshtml.cs`)

Upgrade the login handler to verify the staff member's email address and hashed password against the database, then issue an encrypted Claims authentication cookie:

```csharp
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Data;
using MyRazorApp.Services;

namespace MyRazorApp.Pages.CMS;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _hasher;

    public LoginModel(ApplicationDbContext context, IPasswordHasherService hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    [BindProperty]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // 1. Query staff member by email address
        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => s.Email.ToLower() == Email.Trim().ToLower() && s.IsActive == 1);

        // 2. Verify hashed password
        if (staff == null || string.IsNullOrEmpty(staff.PasswordHash) ||
            !_hasher.VerifyPassword(staff, staff.PasswordHash, Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid email address or password.");
            return Page();
        }

        // 3. Construct user claims identity
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, staff.StaffId.ToString()),
            new(ClaimTypes.Name, $"{staff.FirstName} {staff.LastName}"),
            new(ClaimTypes.Email, staff.Email),
            new(ClaimTypes.Role, staff.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
        };

        // 4. Issue encrypted authentication cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl!)
            : RedirectToPage("/CMS/Index");
    }
}
```

---

## 5. Staff Sign Out (`Pages/CMS/Logout.cshtml.cs`)

With Cookie Authentication, signing out is handled by calling `HttpContext.SignOutAsync`:

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.CMS;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/CMS/Login");
    }
}
```

---

## 6. Granular Access Control with `[Authorize]` & Roles

With Cookie Claims in place, you no longer need custom `CmsPageModel` filter overrides. You can use standard ASP.NET Core `[Authorize]` attributes:

### A. Protecting the Entire CMS Folder
Add a convention in `Program.cs` or decorate each page model with `[Authorize]`:

```csharp
// Authorize all pages in the /CMS folder
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/CMS");
});
```

### B. Restricting Destructive Actions to Admins
To ensure only administrators can delete staff records, decorate `Pages/CMS/Delete.cshtml.cs` with role constraints:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.CMS;

[Authorize(Roles = "Admin")] // Only users with the "Admin" role can access this page
public class DeleteModel : PageModel
{
    // ...
}
```

### C. Conditional UI Display in Razor Views
In `Pages/CMS/Index.cshtml`, conditionally render the "Delete" link only if the signed-in user is an administrator:

```cshtml
<td>
    <a asp-page="Edit" asp-route-id="@member.StaffId">Edit</a>
    @if (User.IsInRole("Admin"))
    {
        <span> | </span>
        <a asp-page="Delete" asp-route-id="@member.StaffId">Delete</a>
    }
</td>
```

---

## 7. Comparison: Session vs. Database Authentication

| Feature | Basic Session Security (`10-basic-authentication.md`) | Database + Hashed Cookie Security (`10-custom-db-authentication.md`) |
| :--- | :--- | :--- |
| **User Identity** | Shared single admin password | Individual staff accounts (Email + Password) |
| **Password Storage** | Configuration file (`appsettings.json`) | Salted PBKDF2 hashes in the `staff` table |
| **State Storage** | Server In-Memory Session | Encrypted, HttpOnly client cookie with Claims |
| **Authorization** | All-or-nothing access | Role-Based Access Control (`Admin`, `Manager`, `Staff`) |
| **Auditability** | None (cannot trace actions to specific users) | High (actions can record `User.Identity.Name` or `StaffId`) |
