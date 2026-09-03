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

        return CryptographicOperations.FixedTimeEquals(configuredBytes, suppliedBytes);
    }

    public void SignIn(HttpContext httpContext) =>
        httpContext.Session.SetString(SessionKey, "true");

    public void SignOut(HttpContext httpContext) =>
        httpContext.Session.Remove(SessionKey);
}