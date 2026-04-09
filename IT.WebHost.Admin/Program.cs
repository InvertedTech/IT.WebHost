using BlazorBlueprint.Components;
using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBlueprintComponents();
builder.Services.AddAuthenticationClasses();
builder.Services.AddCoreServices();
builder.Services.AddGrpcClientClasses();
builder.Services.AddSingleton<ProtoValidate.IValidator, ProtoValidate.Validator>();

var app = builder.Build();
await app.Services.GetRequiredService<SiteSettingsService>().LoadAsync();
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapGet("/auth/logout", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete(JwtExtensions.JWT_COOKIE_NAME);
    return Results.Redirect("/");
});

app.MapBlazorHub();

app.Run();
