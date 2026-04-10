using IT.WebServices.Authentication;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Authorization;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Subscription;

public partial class SettingsSubscriptionTiers
{
    [Inject] private SubscriptionTierHelper SubscriptionTierHelper { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private IList<SubscriptionTier> _data = new List<SubscriptionTier>();

    protected override void OnInitialized()
    {
        _data = SubscriptionTierHelper.GetAll();
    }

    private Task HandleSubmit(IList<SubscriptionTier> tiers)
    {
        // TODO: save tiers
        return Task.CompletedTask;
    }
}
