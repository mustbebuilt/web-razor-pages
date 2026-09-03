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

        _authentication.SignIn(HttpContext);

        return Url.IsLocalUrl(ReturnUrl)
            ? LocalRedirect(ReturnUrl!)
            : RedirectToPage("/CMS/Index");
    }
}