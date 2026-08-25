# Custom Database Authentication with Hashed Passwords

## Overview: Authentication Without ASP.NET Core Identity

While ASP.NET Core provides a full-featured Identity framework (`Microsoft.AspNetCore.Identity.UI`), many custom applications require a lightweight database authentication system without the extra tables, complex DbContext overrides, or scaffolded UI pages.

This guide demonstrates building a **Custom Database Authentication system** using:
1. A custom `User` table/entity in Entity Framework Core.
2. Secure PBKDF2/HMAC-SHA256 password hashing using ASP.NET Core's built-in `PasswordHasher<User>`.
3. Standard ASP.NET Core **Cookie Authentication** (`HttpContext.SignInAsync` and `ClaimsPrincipal`).

---

## 1. Create the User Entity Model (`Models/User.cs`)

Create a database model for application users:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRazorApp.Models;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public string Role { get; set; } = "User";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

Add `DbSet<User> Users` to `Data/ApplicationDbContext.cs`:

```csharp
public DbSet<User> Users { get; set; } = null!;
```

---

## 2. Implement Secure Password Hashing

ASP.NET Core includes `PasswordHasher<TUser>` in `Microsoft.AspNetCore.Identity`, which handles cryptographically secure salt generation and PBKDF2 password hashing automatically.

Create a wrapper service `Services/PasswordHasherService.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
using MyRazorApp.Models;

namespace MyRazorApp.Services;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}

public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password)
    {
        return _hasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
```

> [!IMPORTANT]
> **Never store plain text passwords in a database!** `PasswordHasher<T>` generates a fresh 128-bit random salt for every password and uses 100,000+ PBKDF2 iterations to protect user credentials against rainbow table and brute-force attacks.

---

## 3. Register Cookie Authentication in `Program.cs`

Configure Cookie Authentication services and middleware in `Program.cs`:

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using MyRazorApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Register Password Hasher service
builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();

// Register Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// UseAuthentication MUST be placed before UseAuthorization!
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
```

---

## 4. User Registration Page (`Pages/Account/Register.cshtml.cs`)

When registering a new user:
1. Validate that the username and email are unique.
2. Hash the plain text password using `IPasswordHasherService`.
3. Save the `User` entity to the database.

```csharp
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid) return Page();

    if (_context.Users.Any(u => u.Username.ToLower() == Input.Username.ToLower()))
    {
        ModelState.AddModelError("Input.Username", "Username is already taken.");
        return Page();
    }

    var user = new User
    {
        Username = Input.Username.Trim(),
        Email = Input.Email.Trim().ToLower(),
        Role = "User"
    };

    // Hash plain text password before saving
    user.PasswordHash = _hasher.HashPassword(user, Input.Password);

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    TempData["StatusMessage"] = "Registration successful! Please log in.";
    return RedirectToPage("/Account/Login");
}
```

---

## 5. Login Page & Cookie Issuance (`Pages/Account/Login.cshtml.cs`)

During login:
1. Look up the user in SQLite/SQL Server by username or email.
2. Verify the submitted password against the stored `PasswordHash`.
3. Create a `ClaimsPrincipal` and issue an encrypted HTTP-only authentication cookie using `HttpContext.SignInAsync`.

```csharp
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid) return Page();

    var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == Input.UsernameOrEmail.ToLower());

    // Verify password hash
    if (user == null || !_hasher.VerifyPassword(user, user.PasswordHash, Input.Password))
    {
        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return Page();
    }

    // Create user claims identity
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role)
    };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    // Sign in and issue authentication cookie
    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity));

    return RedirectToPage("/Index");
}
```

---

## 6. User Logout (`Pages/Account/Logout.cshtml.cs`)

To log a user out and invalidate their authentication session, invoke `HttpContext.SignOutAsync`:

```csharp
public async Task<IActionResult> OnGetAsync()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    TempData["StatusMessage"] = "You have been logged out successfully.";
    return RedirectToPage("/Index");
}
```

---

## 7. Protecting Razor Pages with `[Authorize]`

To restrict access to specific Razor Pages, add the `[Authorize]` attribute to the `PageModel`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.Staff;

[Authorize] // Requires authenticated user cookie
public class CreateModel : PageModel
{
    // ...
}
```

In shared layout templates (`_Layout.cshtml`), check authentication state dynamically:

```cshtml
@if (User.Identity != null && User.Identity.IsAuthenticated)
{
    <span>Welcome, <strong>@User.Identity.Name</strong>!</span>
    <a asp-page="/Account/Logout">Logout</a>
}
else
{
    <a asp-page="/Account/Login">Login</a>
    <a asp-page="/Account/Register">Register</a>
}
```
