using IT.Web.Project1.Services;
using IT.WebHost.Core.Config;
using IT.WebServices.Clients.Payment;
using IT.WebServices.Fragments.Authorization;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace WebApp.Components.Pages.Subscribe
{
    public partial class Subscribe
    {
        [Inject] NavigationManager Navigation { get; set; } = null!;
        [Inject] private SiteSettingsService SiteSettings { get; set; } = null!;
        [Inject] private PaymentClient PaymentClient { get; set; } = null!;
        [Inject] private IOptions<AppSettings> _settings { get; set; } = null!;

        private List<SubscriptionTier> tiers { get; set; } = new List<SubscriptionTier>();
        private SubscriptionTier? selectedTier;
        private string zipCode;
        private string selectedProvider = "Stripe";
        private bool isDialogOpen = false;

        protected override void OnInitialized()
        {
            tiers = SiteSettings.Settings?.Subscription?.Tiers.ToList() ?? new();
        }

        public void OpenDialog(SubscriptionTier tier)
        {
            selectedTier = tier;
            isDialogOpen = true;
        }

        public void CloseDialog()
        {
            isDialogOpen = false;
            selectedTier = null;
        }

        public async Task SubmitSubscribe()
        {
            var req = new GetNewDetailsRequest
            {
                Level = selectedTier?.AmountCents != null ? selectedTier.AmountCents : 0,
                PostalCode = zipCode,
                SuccessUrl = $"{_settings.Value.APP_BASE_URL}/subscribe/success",
                CancelUrl = $"{_settings.Value.APP_BASE_URL}/subscribe/cancel"
            };

            // TODO: Figure Out why this says the total amount due is null on service
            var res = await PaymentClient.NewSubscription(
                req
            );

            // TODO: Support Other Providers
            if (selectedProvider == "Stripe" && res is not null && !string.IsNullOrEmpty(res.Stripe.PaymentLink))
            {
                Navigation.NavigateTo(res.Stripe.PaymentLink);
            }

            // TODO: Display Error Messages
        }
    }
}
