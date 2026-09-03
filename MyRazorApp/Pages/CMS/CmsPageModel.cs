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
            context.Result = RedirectToPage("/CMS/Login", new
            {
                returnUrl = Request.Path + Request.QueryString
            });
            return;
        }

        await next();
    }
}