using Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

namespace WebApp.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private ProtectedLocalStorage Storage { get; set; } = null!;
    [Inject] private TemplateService TemplateService { get; set; } = null!;

    private string currentTheme = "default";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        try
        {
            var saved = await Storage.GetAsync<string>("local-theme");
            if (saved.Success && !string.IsNullOrEmpty(saved.Value))
            {
                currentTheme = saved.Value;
                await JS.InvokeVoidAsync("setTheme", currentTheme);
            }
            else
            {
                currentTheme = TemplateService.TemplateFile;
            }
        }
        catch { currentTheme = TemplateService.TemplateFile; }

        StateHasChanged();
    }

    private async Task SetTheme(string name)
    {
        currentTheme = name;
        await JS.InvokeVoidAsync("setTheme", name);
        try { await Storage.SetAsync("local-theme", name); } catch { }
    }
}
