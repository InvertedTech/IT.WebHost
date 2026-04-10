using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace IT.WebHost.CMS.Shared;

public partial class AdminLayout : LayoutComponentBase, IDisposable
{
    private bool _disposed;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private IJSRuntime JS { get; set; } = null!;

    private bool isDarkMode;
    private bool sidebarOpen = false;

    private readonly List<AdminNavItem> navItems = new()
    {
        new() { Text = "Dashboard", Href = "/admin",         Icon = "layout-dashboard" },
        new() { Text = "Content",   Href = "/admin/content", Icon = "file-text" },
        new() { Text = "Assets",    Href = "/admin/assets",  Icon = "image" },
        new() { Text = "Users",     Href = "/admin/users",   Icon = "users" },
    };

    private readonly List<AdminNavItem> settingsNavItems = new()
    {
        new() { Text = "CMS",             Href = "/admin/settings/cms",             Icon = "file" },
        new() { Text = "Merch",           Href = "/admin/settings/merch",           Icon = "shopping-bag" },
        new() { Text = "Personalization", Href = "/admin/settings/personalization", Icon = "palette" },
        new() { Text = "Subscription",    Href = "/admin/settings/subscription",    Icon = "credit-card" },
    };

    protected override void OnInitialized()
    {
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (!_disposed) InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _disposed = true;
        Nav.LocationChanged -= OnLocationChanged;
    }

    private static readonly Dictionary<string, string> SegmentLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"]            = "Admin",
        ["content"]          = "Content",
        ["assets"]           = "Assets",
        ["users"]            = "Users",
        ["settings"]         = "Settings",
        ["cms"]              = "CMS",
        ["merch"]            = "Merch",
        ["personalization"]  = "Personalization",
        ["subscription"]     = "Subscription",
    };

    private List<(string Text, string? Href)> GetBreadcrumbs()
    {
        var segments = new Uri(Nav.Uri).AbsolutePath
            .Trim('/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        var result = new List<(string, string?)>();
        var path = "";
        for (var i = 0; i < segments.Length; i++)
        {
            path += "/" + segments[i];
            var label = SegmentLabels.TryGetValue(segments[i], out var n) ? n : segments[i];
            result.Add((label, i < segments.Length - 1 ? path : null));
        }
        return result;
    }

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

        if (!user.Claims.Any(c => c.Type == System.Security.Claims.ClaimTypes.Role))
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
