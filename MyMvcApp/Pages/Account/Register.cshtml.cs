using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyMvcApp.Data;
using MyMvcApp.Models;
using MyMvcApp.Services;

namespace MyMvcApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _hasher;

    public RegisterModel(ApplicationDbContext context, IPasswordHasherService hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _context.Database.EnsureCreatedAsync();

        if (_context.Users.Any(u => u.Username.ToLower() == Input.Username.ToLower()))
        {
            ModelState.AddModelError("Input.Username", "Username is already taken.");
            return Page();
        }

        if (_context.Users.Any(u => u.Email.ToLower() == Input.Email.ToLower()))
        {
            ModelState.AddModelError("Input.Email", "Email address is already registered.");
            return Page();
        }

        var user = new User
        {
            Username = Input.Username.Trim(),
            Email = Input.Email.Trim().ToLower(),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _hasher.HashPassword(user, Input.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Registration successful! You can now log in.";
        return RedirectToPage("/Account/Login");
    }
}
