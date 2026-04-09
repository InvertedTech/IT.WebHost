using BlazorBlueprint.Components;
using IT.WebHost.CMS;
using IT.WebHost.CMS.Auth;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBlueprintComponents();
builder.Services.AddGrpcClientClasses();
builder.Services.AddCoreServices();
builder.Services.AddAuthenticationClasses();
builder.Services.AddSingleton<AuthenticationStateProvider, StubAuthenticationStateProvider>();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
