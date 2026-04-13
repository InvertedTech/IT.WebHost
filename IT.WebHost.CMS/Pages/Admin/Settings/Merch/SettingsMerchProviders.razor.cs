using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Merch;
using IT.WebServices.Fragments.Merch.Shopify;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Merch;

public partial class SettingsMerchProviders
{
    [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _isEditing = false;
    private bool _shopifyEnabled;
    private ShopifyOwnerSettings _shopifySettings = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var pub = await PublicSettingsClient.PublicData;
        _shopifyEnabled = pub.Merch?.Shopify?.IsEnabled ?? false;

        var owner = await SettingsClient.OwnerData;
        _shopifySettings = owner.Merch?.Shopify ?? new ShopifyOwnerSettings();
    }

    private void StartEdit() => _isEditing = true;

    private async Task CancelEdit()
    {
        _isEditing = false;
        await LoadAsync();
    }

    private async Task HandleShopifySubmit(ModifyMerchOwnerSettingsRequest request)
    {
        await SettingsClient.ModifyMerchOwnerSettings(request);
    }

    private async Task HandleSave()
    {
        await HandleShopifySubmit(new ModifyMerchOwnerSettingsRequest
        {
            Data = new MerchOwnerSettings { Shopify = _shopifySettings }
        });
        _isEditing = false;
    }
}
