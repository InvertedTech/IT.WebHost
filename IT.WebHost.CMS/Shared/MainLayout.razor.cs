using BlazorBlueprint.Components;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace IT.WebHost.CMS.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private ProtectedLocalStorage Storage { get; set; } = null!;
    [Inject] private TemplateService TemplateService { get; set; } = null!;

    private bool isDarkMode;
    private string currentTheme = "default";

    public readonly List<NavItemDefinition> navItems = new()
    {
        new() { Text = "Home",         Href = "/",             Match = NavLinkMatch.All },
        new() { Text = "Subscribe",    Href = "/subscribe" },
        new() { Text = "Members Area", Href = "/members-area" },
        new() { Text = "Search",       Href = "/search" },
        new() { Text = "Merch",        Href = "/merch" },
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        isDarkMode = SiteSettings.Settings?.Personalization?.DefaultToDarkMode ?? false;
        await JS.InvokeVoidAsync("setDarkMode", isDarkMode);

        try
        {
            var saved = await Storage.GetAsync<string>("local-theme");
            if (saved.Success && !string.IsNullOrEmpty(saved.Value))
            {
                currentTheme = saved.Value;
                await JS.InvokeVoidAsync("setTheme", currentTheme);
            }
            else
            {
                currentTheme = TemplateService.TemplateFile;
            }
        }
        catch { currentTheme = TemplateService.TemplateFile; }

        StateHasChanged();
    }

    private async Task ToggleDarkMode()
    {
        await JS.InvokeVoidAsync("toggleDarkMode");
        isDarkMode = !isDarkMode;
    }

    private async Task SetTheme(string name)
    {
        currentTheme = name;
        await JS.InvokeVoidAsync("setTheme", name);
        try { await Storage.SetAsync("local-theme", name); } catch { }
    }
}
