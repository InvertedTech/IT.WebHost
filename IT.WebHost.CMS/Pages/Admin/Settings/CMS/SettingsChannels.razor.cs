using IT.WebServices.Authentication;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.CMS;

public partial class SettingsChannels
{
    [Inject] private ChannelHelper ChannelHelper { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private IEnumerable<ChannelRecord> channels = new List<ChannelRecord>();

    protected override async Task OnInitializedAsync()
    {
        await LoadChannelsAsync();
    }

    private async Task LoadChannelsAsync()
    {
        var foundChannels = ChannelHelper.GetAll();
        if (foundChannels != null)
        {
            channels = foundChannels;
        }
    }
    private Task SaveChannelAsync(ChannelRecord channel) => throw new NotImplementedException();
}
