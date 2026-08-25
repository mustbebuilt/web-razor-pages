using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;
using MyMvcApp.Services;

namespace MyMvcApp.Pages.Account;

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
    public InputModel Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Username or email is required.")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _context.Database.EnsureCreatedAsync();

        var inputLower = Input.UsernameOrEmail.Trim().ToLower();
        var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == inputLower || u.Email.ToLower() == inputLower);

        if (user == null || !_hasher.VerifyPassword(user, user.PasswordHash, Input.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid login credentials.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = Input.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        TempData["StatusMessage"] = $"Welcome back, {user.Username}!";
        return LocalRedirect(returnUrl);
    }
}
