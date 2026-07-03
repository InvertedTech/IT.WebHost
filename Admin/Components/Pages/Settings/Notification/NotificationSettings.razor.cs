using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Settings.Notification
{
    public partial class NotificationSettings
    {
        [Inject] SettingsClient SettingsClient { get; set; } = null!;
        [Inject] IToastService ToastService { get; set; } = null!;

        private NotificationOwnerRecord _notificationOwnerSettings { get; set; } = new()
        {
            Sendgrid = new()
        };

        private bool IsEditing { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
        }

        private async Task LoadSettings()
        {
            var res = await SettingsClient.OwnerData;

            if (res != null)
            {
                _notificationOwnerSettings = res.Notification ?? new NotificationOwnerRecord();
                _notificationOwnerSettings.Sendgrid ??= new();
            }
        }

        private void StartEdit() => IsEditing = true;

        private async Task CancelEdit()
        {
            IsEditing = false;
            await LoadSettings();
        }

        private async Task SaveSettingsAsync()
        {
            var req = new ModifyNotificationOwnerDataRequest
            {
                Data = _notificationOwnerSettings
            };

            var res = await SettingsClient.ModifyNotificationOwnerSettings(req);

            if (res is null || res.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("Notification settings saved successfully.");
                IsEditing = false;
                await LoadSettings();
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Message)
                    ? res.Message
                    : res.Reason.ToString();

                ToastService.Error(message);
            }
        }
    }
}
