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
    private bool _isEditing = false;

    protected override void OnInitialized()
    {
        LoadData();
    }

    private void LoadData()
    {
        _data = SubscriptionTierHelper.GetAll();
    }

    private void StartEdit() => _isEditing = true;

    private void CancelEdit()
    {
        _isEditing = false;
        LoadData();
    }

    private void HandleSave() => _isEditing = false;

    private Task HandleSubmit(IList<SubscriptionTier> tiers)
    {
        // TODO: save tiers
        _isEditing = false;
        return Task.CompletedTask;
    }
}
