using BlazorBlueprint.Components;
using IT.WebHost.CMS;
using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBlueprintComponents();
builder.Services.AddGrpcClientClasses();
builder.Services.AddCoreServices();
builder.Services.AddAuthenticationClasses();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

await app.Services.GetRequiredService<SiteSettingsService>().LoadAsync();

app.MapStaticAssets();

app.MapGet("/auth/set-cookie", (string token, string? returnUrl, HttpContext ctx) =>
{
    ctx.Response.Cookies.Append(JwtExtensions.JWT_COOKIE_NAME, token, new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/"
    });
    return Results.Redirect(returnUrl ?? "/");
});

app.MapGet("/auth/logout", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete(JwtExtensions.JWT_COOKIE_NAME);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
