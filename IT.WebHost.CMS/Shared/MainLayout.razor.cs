using BlazorBlueprint.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace IT.WebHost.CMS.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private bool isDarkMode;

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
        if (firstRender)
        {
            isDarkMode = SiteSettings.Settings?.Personalization?.DefaultToDarkMode ?? false;
            await JS.InvokeVoidAsync("setDarkMode", isDarkMode);
            StateHasChanged();
        }
    }

    private async Task ToggleDarkMode()
    {
        await JS.InvokeVoidAsync("toggleDarkMode");
        isDarkMode = !isDarkMode;
    }
}
