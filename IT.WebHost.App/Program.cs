using BlazorBlueprint.Components;
using IT.WebHost.Core.Services;
using IT.WebHost.Core.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthenticationClasses();
builder.Services.AddCoreClients();
builder.Services.AddCoreServices();

// Add BlazorBlueprint services
builder.Services.AddBlazorBlueprintComponents();
builder.Services.AddSingleton<ProtoValidate.IValidator, ProtoValidate.Validator>();

var app = builder.Build();

await app.Services.GetRequiredService<SiteSettingsService>().LoadAsync();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "members-area",
    pattern: "members-area",
    defaults: new { controller = "Members", action = "Index" })
    .WithStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapBlazorHub();

app.MapGet("/auth/set-cookie", (string token, string? returnUrl, HttpContext ctx) =>
{
    ctx.Response.Cookies.Append(AuthClient.CookieName, token, new CookieOptions
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
    ctx.Response.Cookies.Delete(AuthClient.CookieName);
    return Results.Redirect("/");
});

app.Run();
