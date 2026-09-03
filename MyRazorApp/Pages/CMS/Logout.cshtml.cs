using Microsoft.AspNetCore.Mvc;

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