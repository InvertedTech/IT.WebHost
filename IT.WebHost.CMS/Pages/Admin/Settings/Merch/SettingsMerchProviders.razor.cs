using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Merch.Shopify;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Merch;

public partial class SettingsMerchProviders
{
    [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _shopifyEnabled;
    private ShopifyOwnerSettings _shopifySettings = new();

    protected override async Task OnInitializedAsync()
    {
        var pub = await PublicSettingsClient.PublicData;
        _shopifyEnabled = pub.Merch?.Shopify?.IsEnabled ?? false;

        var owner = await SettingsClient.OwnerData;
        _shopifySettings = owner.Merch?.Shopify ?? new ShopifyOwnerSettings();
    }

    private async Task HandleShopifySubmit(ModifyMerchOwnerSettingsRequest request)
    {
        await SettingsClient.ModifyMerchOwnerSettings(request);
    }
}
