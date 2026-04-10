using BlazorBlueprint.Primitives.DashboardGrid;
using IT.WebHost.CMS.Services;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Dashboard
{
    [Inject] private DashboardLayoutService LayoutService { get; set; } = default!;

    private bool isEditing = false;
    private bool isAddWidgetOpen = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        await LayoutService.LoadAsync();
        StateHasChanged();
    }

    private void HandleAddWidget() => isAddWidgetOpen = true;

    private async Task AddWidget(string widgetId)
    {
        LayoutService.State.AddWidget(widgetId);
        isAddWidgetOpen = false;
        await LayoutService.SaveAsync();
    }

    private async Task RemoveWidget(string widgetId)
    {
        LayoutService.State.RemoveWidget(widgetId);
        await LayoutService.SaveAsync();
    }

    private async Task HandleDragEnd(WidgetDragEventArgs args)
    {
        var p = args.NewPosition;
        LayoutService.State.UpdatePosition(args.WidgetId, p.Column, p.Row, p.ColumnSpan, p.RowSpan);
        await LayoutService.SaveAsync();
    }

    private async Task HandleResizeEnd(WidgetResizeEventArgs args)
    {
        var p = args.NewPosition;
        LayoutService.State.UpdatePosition(args.WidgetId, p.Column, p.Row, p.ColumnSpan, p.RowSpan);
        await LayoutService.SaveAsync();
    }
}
