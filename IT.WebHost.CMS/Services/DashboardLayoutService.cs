using IT.WebHost.CMS.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace IT.WebHost.CMS.Services;

public class DashboardLayoutService(ProtectedLocalStorage storage, WidgetRegistryService registry)
{
    private const string StorageKey = "dashboard-layout";

    public DashboardLayoutState State { get; } = new();

    public async Task LoadAsync()
    {
        try
        {
            var result = await storage.GetAsync<List<DashboardWidgetConfig>>(StorageKey);
            if (result.Success && result.Value is { Count: > 0 })
                State.Widgets = result.Value;
            else
                State.Widgets = registry.GetDefaultLayout().ToList();
        }
        catch
        {
            State.Widgets = registry.GetDefaultLayout().ToList();
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            await storage.SetAsync(StorageKey, State.Widgets);
        }
        catch { }
    }
}
