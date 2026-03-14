using IT.WebHost.App;
using BlazorBlueprint.Components;
using IT.WebHost.App.Services;
using IT.WebHost.Core.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<TemplateService>();
builder.Services.AddHttpClient("content");
builder.Services.AddSingleton<ContentClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var baseUrl = builder.Configuration["ApiUrl"] ?? "http://localhost:8001/api";
    return new ContentClient(factory.CreateClient("content"), baseUrl);
});

// Add BlazorBlueprint services
builder.Services.AddBlazorBlueprintComponents();

var app = builder.Build();

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
