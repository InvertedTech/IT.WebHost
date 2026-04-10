using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Subscribe
{
    [Inject] private SiteSettingsService SiteSettings { get; set; } = null!;

    private SubscribeViewModel _model = new();

    protected override void OnInitialized()
    {
        _model = new SubscribeViewModel
        {
            Tiers = SiteSettings.Settings?.Subscription?.Tiers ?? new()
        };
    }
}
