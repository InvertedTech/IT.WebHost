using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Authorization.Payment.Fortis;
using IT.WebServices.Fragments.Authorization.Payment.Paypal;
using IT.WebServices.Fragments.Authorization.Payment.Stripe;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Subscription;

public partial class SettingsSubscriptionProviders
{
    [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _isEditing = false;
    private bool _manualEnabled;
    private bool _stripeEnabled;
    private bool _paypalEnabled;
    private bool _cryptoEnabled;
    private bool _fortisEnabled;

    private FortisOwnerSettings _fortisSettings = new();
    private StripeOwnerSettings _stripeSettings = new();
    private PaypalOwnerSettings _paypalSettings = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var pub = await PublicSettingsClient.PublicData;
        var sub = pub.Subscription;
        if (sub is not null)
        {
            _manualEnabled = sub.Manual?.Enabled ?? false;
            _stripeEnabled = sub.Stripe?.Enabled ?? false;
            _paypalEnabled = sub.Paypal?.Enabled ?? false;
            _cryptoEnabled = sub.Crypto?.Enabled ?? false;
            _fortisEnabled = sub.Fortis?.Enabled ?? false;
        }

        var owner = await SettingsClient.OwnerData;
        var ownerSub = owner.Subscription;
        if (ownerSub is not null)
        {
            _fortisSettings = ownerSub.Fortis ?? new FortisOwnerSettings();
            _stripeSettings = ownerSub.Stripe ?? new StripeOwnerSettings();
            _paypalSettings = ownerSub.Paypal ?? new PaypalOwnerSettings();
        }
    }

    private void StartEdit() => _isEditing = true;

    private async Task CancelEdit()
    {
        _isEditing = false;
        await LoadAsync();
    }

    private async Task HandleFortisSubmit(FortisOwnerSettings settings)
    {
        var owner = await SettingsClient.OwnerData;
        var record = (owner.Subscription ?? new SubscriptionOwnerRecord()).Clone();
        record.Fortis = settings;
        await SettingsClient.ModifySubscriptionOwnerSettings(new ModifySubscriptionOwnerDataRequest { Data = record });
    }

    private async Task HandleStripeSubmit(StripeOwnerSettings settings)
    {
        var owner = await SettingsClient.OwnerData;
        var record = (owner.Subscription ?? new SubscriptionOwnerRecord()).Clone();
        record.Stripe = settings;
        await SettingsClient.ModifySubscriptionOwnerSettings(new ModifySubscriptionOwnerDataRequest { Data = record });
    }

    private async Task HandlePaypalSubmit(PaypalOwnerSettings settings)
    {
        var owner = await SettingsClient.OwnerData;
        var record = (owner.Subscription ?? new SubscriptionOwnerRecord()).Clone();
        record.Paypal = settings;
        await SettingsClient.ModifySubscriptionOwnerSettings(new ModifySubscriptionOwnerDataRequest { Data = record });
    }

    private async Task HandleSave()
    {
        await HandleStripeSubmit(_stripeSettings);
        await HandlePaypalSubmit(_paypalSettings);
        await HandleFortisSubmit(_fortisSettings);
        _isEditing = false;
    }
}
