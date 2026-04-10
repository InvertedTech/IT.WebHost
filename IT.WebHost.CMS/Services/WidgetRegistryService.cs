using IT.WebHost.CMS.Models;

namespace IT.WebHost.CMS.Services;

public class WidgetRegistryService
{
    private readonly List<WidgetDescriptor> _widgets = [];
    private readonly List<DashboardWidgetConfig> _defaultLayout = [];

    public void Register(WidgetDescriptor descriptor) => _widgets.Add(descriptor);

    public void RegisterDefault(DashboardWidgetConfig config) => _defaultLayout.Add(config);

    public IReadOnlyList<WidgetDescriptor> GetAll() => _widgets;
    public WidgetDescriptor? Get(string id) => _widgets.FirstOrDefault(w => w.Id == id);
    public IReadOnlyList<DashboardWidgetConfig> GetDefaultLayout() => _defaultLayout;
}
