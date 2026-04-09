using IT.WebServices.Fragments.Settings;

namespace IT.WebHost.Admin.Models.Settings.Cms
{
    public class SettingsCmsChannelsViewModel
    {
        public IEnumerable<ChannelRecord> Channels { get; set; } = new List<ChannelRecord>();
    }
}
