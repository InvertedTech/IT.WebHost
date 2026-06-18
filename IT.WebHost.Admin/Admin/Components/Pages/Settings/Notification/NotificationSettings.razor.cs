using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Settings.Notification
{
    public partial class NotificationSettings
    {
        [Inject] SettingsClient SettingsClient { get; set; } = null!;

        private NotificationOwnerRecord _notificationOwnerSettings { get; set; } = new()
        {
            Sendgrid = new()
        };

        protected override async Task OnInitializedAsync()
        {
            // TODO: Requires owner role for SettingsClient.OwnerData (GetOwnerData route); note AuthorizeView needed in .razor
            await LoadSettings();
        }

        private async Task LoadSettings()
        {
            var res = await SettingsClient.OwnerData;

            if (res != null)
            {
                _notificationOwnerSettings = res.Notification;
            }
        }
    }
}
