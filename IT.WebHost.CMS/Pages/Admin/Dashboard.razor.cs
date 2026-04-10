using IT.WebHost.CMS.Models;
using static BlazorBlueprint.Components.BbDashboardGrid;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Dashboard
{
    private bool isEditing = false;
    private bool isAddWidgetOpen = false;

    private List<IT.WebHost.CMS.Models.DashboardWidgetConfig> Widgets { get; set; } =
    [
        new() { WidgetId = "stats-cards", Col = 1,  Row = 1, ColSpan = 24, RowSpan = 4 },
        new() { WidgetId = "top-content", Col = 1,  Row = 5, ColSpan = 12, RowSpan = 6 },
    ];

    private void HandleAddWidget() => isAddWidgetOpen = true;

    private void AddWidget(string widgetId)
    {
        if (Widgets.Any(w => w.WidgetId == widgetId)) return;

        var nextRow = Widgets.Count > 0 ? Widgets.Max(w => w.Row + w.RowSpan) : 1;
        Widgets.Add(new() { WidgetId = widgetId, Col = 1, Row = nextRow, ColSpan = 12, RowSpan = 4 });
        isAddWidgetOpen = false;
    }

    private void RemoveWidget(string widgetId) =>
        Widgets.RemoveAll(w => w.WidgetId == widgetId);
}
