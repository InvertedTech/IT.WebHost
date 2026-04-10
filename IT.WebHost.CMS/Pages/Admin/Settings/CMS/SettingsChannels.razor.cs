using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.CMS;

public partial class SettingsChannels
{
    [Inject] private SettingsInterface.SettingsInterfaceClient SettingsClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private IEnumerable<ChannelRecord> channels = new List<ChannelRecord>();

    private Task LoadChannelsAsync() => throw new NotImplementedException();
    private Task SaveChannelAsync(ChannelRecord channel) => throw new NotImplementedException();
}
