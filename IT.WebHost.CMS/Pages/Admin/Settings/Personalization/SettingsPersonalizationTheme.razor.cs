using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Personalization;

public partial class SettingsPersonalizationTheme
{
    [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _defaultToDarkMode;

    protected override async Task OnInitializedAsync()
    {
        var data = await PublicSettingsClient.PublicData;
        _defaultToDarkMode = data.Personalization?.DefaultToDarkMode ?? false;
    }

    private async Task HandleSubmit()
    {
        var data = await PublicSettingsClient.PublicData;
        var record = (data.Personalization ?? new PersonalizationPublicRecord()).Clone();
        record.DefaultToDarkMode = _defaultToDarkMode;

        await SettingsClient.ModifyPersonalizationPublicSettings(new ModifyPersonalizationPublicDataRequest { Data = record });
    }
}
