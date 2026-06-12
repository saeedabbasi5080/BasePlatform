namespace BasePlatform.Admin.Configuration;

public static class AdminCookieDefaults
{
    public const string AuthenticationScheme = "AdminCookie";
    public const string LoginPath = "/admin/auth/login";
    public const string AccessDeniedPath = "/admin/auth/access-denied";
}