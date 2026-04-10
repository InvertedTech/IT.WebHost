using IT.WebHost.CMS.Models;
using static BlazorBlueprint.Components.BbDashboardGrid;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Dashboard
{
    private bool isEditing = false;

    private List<IT.WebHost.CMS.Models.DashboardWidgetConfig> Widgets { get; set; } =
    [
        new() { WidgetId = "stats-cards", Col = 1,  Row = 1, ColSpan = 24, RowSpan = 4 },
        new() { WidgetId = "top-content", Col = 1,  Row = 5, ColSpan = 12, RowSpan = 6 },
    ];

    private void HandleAddWidget() { }

    private void RemoveWidget(string widgetId) { }
}
