using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace IT.WebHost.CMS.Shared;

public partial class AdminLayout : LayoutComponentBase
{
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private bool isDarkMode;

    private readonly List<AdminNavItem> navItems = new()
    {
        new() { Text = "Dashboard", Href = "/admin",          Icon = "layout-dashboard" },
        new() { Text = "Content",   Href = "/admin/content",  Icon = "file-text" },
        new() { Text = "Users",     Href = "/admin/users",    Icon = "users" },
        new() { Text = "Settings",  Href = "/admin/settings", Icon = "settings" },
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            Nav.NavigateTo("/login", forceLoad: false);
            return;
        }

        if (!user.IsInRole("Admin"))
        {
            Nav.NavigateTo("/", forceLoad: false);
            return;
        }

        isDarkMode = await JS.InvokeAsync<bool>("isDarkModeEnabled");
        StateHasChanged();
    }

    private async Task ToggleDarkMode()
    {
        await JS.InvokeVoidAsync("toggleDarkMode");
        isDarkMode = !isDarkMode;
    }

    private record AdminNavItem
    {
        public string Text { get; init; } = "";
        public string Href { get; init; } = "";
        public string Icon { get; init; } = "";
    }
}
