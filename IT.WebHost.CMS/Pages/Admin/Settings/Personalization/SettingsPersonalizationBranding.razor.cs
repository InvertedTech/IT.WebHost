using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Personalization;

public partial class SettingsPersonalizationBranding
{
    [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private string _title = string.Empty;
    private string _metaDescription = string.Empty;
    private string _profileImageAssetId = string.Empty;
    private string _headerImageAssetId = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var data = await PublicSettingsClient.PublicData;
        var p = data.Personalization;
        if (p is null) return;

        _title = p.Title;
        _metaDescription = p.MetaDescription;
        _profileImageAssetId = p.ProfileImageAssetId;
        _headerImageAssetId = p.HeaderImageAssetId;
    }

    private async Task HandleSubmit()
    {
        var data = await PublicSettingsClient.PublicData;
        var record = (data.Personalization ?? new PersonalizationPublicRecord()).Clone();
        record.Title = _title;
        record.MetaDescription = _metaDescription;
        record.ProfileImageAssetId = _profileImageAssetId;
        record.HeaderImageAssetId = _headerImageAssetId;

        await SettingsClient.ModifyPersonalizationPublicSettings(new ModifyPersonalizationPublicDataRequest { Data = record });
    }
}
