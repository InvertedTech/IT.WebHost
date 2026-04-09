using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Merch;

public partial class SettingsMerch
{
    [Inject] private SettingsInterface.SettingsInterfaceClient SettingsClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private Task LoadSettingsAsync() => throw new NotImplementedException();
    private Task SaveMerchSettingsAsync() => throw new NotImplementedException();
}
