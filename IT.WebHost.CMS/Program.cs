using BlazorBlueprint.Components;
using IT.WebHost.CMS;
using IT.WebHost.CMS.Components.Admin.Dashboard;
using IT.WebHost.CMS.Services;
using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents().AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 4000000;
    });
builder.Services.AddBlazorBlueprintComponents();
builder.Services.AddGrpcClientClasses();
builder.Services.AddCoreServices();
builder.Services.AddAuthenticationClasses();
builder.Services.AddScoped<DashboardLayoutService>();
builder.Services.AddSingleton<WidgetRegistryService>(sp => {
    var registry = new WidgetRegistryService();
    registry.Register(new("stats-cards", "Stats Cards", "bar-chart-2", typeof(StatsCardsWidget)));
    registry.Register(new("top-content", "Top Content", "film", typeof(TopContentWidget)));
    registry.Register(new("user-stats", "User Stats", "users", typeof(UserStatsWidget)));
    registry.Register(new("top-plans", "Top Plans by Revenue", "trophy", typeof(TopPlansWidget)));
    registry.Register(new("content-engagement", "Content Engagement", "activity", typeof(ContentEngagementWidget)));

    registry.RegisterDefault(new() { WidgetId = "stats-cards", Col = 1, Row = 1, ColSpan = 24, RowSpan = 4 });
    registry.RegisterDefault(new() { WidgetId = "top-content", Col = 1, Row = 5, ColSpan = 12, RowSpan = 6 });
    return registry;
});


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

app.MapGet("/logout", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete(JwtExtensions.JWT_COOKIE_NAME);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
