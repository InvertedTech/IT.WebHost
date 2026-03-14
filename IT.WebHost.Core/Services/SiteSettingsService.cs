using IT.WebHost.Core.Clients;
using IT.WebServices.Fragments.Settings;

namespace IT.WebHost.Core.Services
{
    public class SiteSettingsService(SettingsClient settingsClient)
    {
        public SettingsPublicData? Settings { get; private set; }

        public string SiteTitle => Settings?.Personalization?.Title ?? "IT.WebHost";

        public async Task LoadAsync()
        {
            try
            {
                Settings = await settingsClient.GetPublicDataAsync();
            }
            catch
            {
                // Fallback to defaults if settings unavailable at startup
            }
        }
    }
}
