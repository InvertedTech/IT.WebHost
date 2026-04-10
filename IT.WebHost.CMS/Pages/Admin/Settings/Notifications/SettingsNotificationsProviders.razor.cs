using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Notification;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Notifications;

public partial class SettingsNotificationsProviders
{
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _sendgridEnabled;
    private SendgridOwnerSettings _sendgridSettings = new();

    protected override async Task OnInitializedAsync()
    {
        var owner = await SettingsClient.OwnerData;
        var sendgrid = owner.Notification?.Sendgrid;
        if (sendgrid is null) return;

        _sendgridEnabled = sendgrid.Enabled;
        _sendgridSettings = sendgrid.Clone();
    }

    private async Task HandleSendgridSubmit(SendgridOwnerSettings settings)
    {
        var owner = await SettingsClient.OwnerData;
        var record = (owner.Notification ?? new NotificationOwnerRecord()).Clone();
        record.Sendgrid = settings;
        record.Sendgrid.Enabled = _sendgridEnabled;

        await SettingsClient.ModifyNotificationOwnerSettings(new ModifyNotificationOwnerDataRequest { Data = record });
    }
}
