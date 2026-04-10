using IT.WebHost.CMS.Models;

namespace IT.WebHost.CMS.Services;

public class DashboardLayoutState
{
    public List<DashboardWidgetConfig> Widgets { get; set; } = [];

    public void AddWidget(string widgetId)
    {
        if (Widgets.Any(w => w.WidgetId == widgetId)) return;
        var nextRow = Widgets.Count > 0 ? Widgets.Max(w => w.Row + w.RowSpan) : 1;
        Widgets.Add(new() { WidgetId = widgetId, Col = 1, Row = nextRow, ColSpan = 12, RowSpan = 4 });
    }

    public void RemoveWidget(string widgetId) =>
        Widgets.RemoveAll(w => w.WidgetId == widgetId);

    public void UpdatePosition(string widgetId, int col, int row, int colSpan, int rowSpan)
    {
        var widget = Widgets.FirstOrDefault(w => w.WidgetId == widgetId);
        if (widget is null) return;

        widget.Col = col;
        widget.Row = row;
        widget.ColSpan = colSpan;
        widget.RowSpan = rowSpan;
    }
}
