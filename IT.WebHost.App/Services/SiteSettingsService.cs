using IT.WebHost.Core.Clients;
using IT.WebServices.Fragments.Settings;

namespace IT.WebHost.App.Services
{
    public class SiteSettingsService
    {
        private readonly SettingsClient _settingsClient;

        public SettingsPublicData? Settings { get; private set; }

        public string SiteTitle => Settings?.Personalization?.Title ?? "IT.WebHost";

        public SiteSettingsService(SettingsClient settingsClient)
        {
            _settingsClient = settingsClient;
        }

        public async Task LoadAsync()
        {
            try
            {
                Settings = await _settingsClient.GetPublicDataAsync();
            }
            catch
            {
                // Fallback to defaults if settings unavailable at startup
            }
        }
    }
}
