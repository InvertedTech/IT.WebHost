using IT.WebHost.App;
using BlazorBlueprint.Components;
using IT.WebHost.App.Services;
using IT.WebHost.Core.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TemplateService>();
builder.Services.AddHttpClient("content");
builder.Services.AddSingleton<ContentClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var baseUrl = builder.Configuration["ApiUrl"] ?? "http://localhost:8001/api";
    return new ContentClient(factory.CreateClient("content"), baseUrl);
});

builder.Services.AddHttpClient("auth");
builder.Services.AddSingleton<AuthClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var baseUrl = builder.Configuration["ApiUrl"] ?? "http://localhost:8001/api";
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    return new AuthClient(factory.CreateClient("auth"), baseUrl, httpContextAccessor);
});

builder.Services.AddHttpClient("settings");
builder.Services.AddSingleton<SettingsClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var baseUrl = builder.Configuration["ApiUrl"] ?? "http://localhost:8001/api";
    return new SettingsClient(factory.CreateClient("settings"), baseUrl);
});
builder.Services.AddSingleton<SiteSettingsService>();

// Add BlazorBlueprint services
builder.Services.AddBlazorBlueprintComponents();

var app = builder.Build();

await app.Services.GetRequiredService<SiteSettingsService>().LoadAsync();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
