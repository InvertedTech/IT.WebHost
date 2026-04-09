using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Personalization;

public partial class SettingsPersonalization
{
    [Inject] private SettingsInterface.SettingsInterfaceClient SettingsClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private Task LoadSettingsAsync() => throw new NotImplementedException();
    private Task SavePersonalizationSettingsAsync() => throw new NotImplementedException();
}
