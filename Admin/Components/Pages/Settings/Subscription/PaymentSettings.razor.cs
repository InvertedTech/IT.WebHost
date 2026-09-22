using IT.WebServices.Authentication;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Authorization;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Settings.Subscription
{
    public partial class PaymentSettings
    {
        [Inject] public PublicSettingsClient publicSettings { get; set; } = null!;
        [Inject] public SettingsInterface.SettingsInterfaceClient settingsClient { get; set; } = null!;
        [Inject] public ONUserHelper userHelper { get; set; } = null!;
        [Inject] public IToastService ToastService { get; set; } = null!;

        private SubscriptionPublicRecord _public { get; set; } = new()
        {
            Manual = new(),
            Fortis = new(),
            Crypto = new(),
            Stripe = new(),
            Paypal = new()
        };

        private SubscriptionOwnerRecord _owner { get; set; } = new()
        {
            Stripe = new(),
            Paypal = new(),
            Fortis = new()
        };


        protected override async Task OnInitializedAsync()
        {
            var pubSettings = await publicSettings.PublicData;
            _public = pubSettings.Subscription ?? new SubscriptionPublicRecord();
            _public.Manual ??= new();
            _public.Fortis ??= new();
            _public.Crypto ??= new();
            _public.Stripe ??= new();
            _public.Paypal ??= new();

            if (userHelper.MyUser?.RoleAbilities.IsOwner == true)
            {
                var ownerSettings = await settingsClient.GetOwnerDataAsync(new GetOwnerDataRequest(), userHelper.GetGrpcCallOptions());
                _owner = ownerSettings.Owner.Subscription ?? new SubscriptionOwnerRecord();
                _owner.Stripe ??= new();
                _owner.Paypal ??= new();
                _owner.Fortis ??= new();
            }
        }

        private bool _isEditing = false;

        private void ToggleEdit() => _isEditing = !_isEditing;

        private async Task CancelEdit()
        {
            _isEditing = false;
            await OnInitializedAsync();
        }

        private async Task HandleSave()
        {
            try
            {
                var pubRes = await settingsClient.ModifySubscriptionPublicDataAsync(new ModifySubscriptionPublicDataRequest
                {
                    Data = _public
                }, userHelper.GetGrpcCallOptions());

                if (pubRes.Error != null && pubRes.Error.Reason != IT.WebServices.Fragments.APIErrorReason.ErrorReasonNoError)
                {
                    ToastService.Error($"Failed to save subscription settings: {pubRes.Error.Reason}");
                    return;
                }

                if (userHelper.MyUser?.RoleAbilities.IsOwner == true)
                {
                    var ownerRes = await settingsClient.ModifySubscriptionOwnerDataAsync(new ModifySubscriptionOwnerDataRequest
                    {
                        Data = _owner
                    }, userHelper.GetGrpcCallOptions());

                    if (ownerRes.Error != null && ownerRes.Error.Reason != IT.WebServices.Fragments.APIErrorReason.ErrorReasonNoError)
                    {
                        ToastService.Error($"Settings saved, but processor credentials failed: {ownerRes.Error.Reason}");
                        _isEditing = false;
                        return;
                    }
                }

                ToastService.Success("Subscription settings saved successfully.");
                _isEditing = false;
            }
            catch (Exception)
            {
                ToastService.Error("Failed to save subscription settings.");
            }
        }

        private void OnCreateTier(SubscriptionTier tier)
        {
            _public.Tiers.Add(tier);
        }

        private void OnEditTier((int Index, SubscriptionTier Tier) args)
        {
            if (args.Index < 0 || args.Index >= _public.Tiers.Count)
                return;

            _public.Tiers[args.Index] = args.Tier;
        }

        private void OnDeleteTier(int index)
        {
            if (index < 0 || index >= _public.Tiers.Count)
                return;

            _public.Tiers.RemoveAt(index);
        }
    }
}
