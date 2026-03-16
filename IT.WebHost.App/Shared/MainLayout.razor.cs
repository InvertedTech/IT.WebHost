using BlazorBlueprint.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace IT.WebHost.App.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private IJSRuntime JS { get; set; } = null!;

    private bool isDarkMode;

    public readonly List<NavItemDefinition> navItems = new()
    {
        new() { Text = "Home", Href = "/" },
        new() { Text = "Subscribe", Href = "/subscribe" },
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
