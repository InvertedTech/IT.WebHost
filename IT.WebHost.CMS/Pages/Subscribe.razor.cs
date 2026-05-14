using IT.WebHost.Core.Models;
using IT.WebHost.Core.Services;
using IT.WebServices.Clients.Payment;
using IT.WebServices.Fragments.Authorization;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Subscribe
{
    [Inject] NavigationManager Navigation { get; set; } = null!;
    [Inject] private SiteSettingsService SiteSettings { get; set; } = null!;
    [Inject] private PaymentClient PaymentClient { get; set; } = null!;

    private SubscribeViewModel _model = new();
    private SubscriptionTier? selectedTier;
    private string zipCode;
    private string selectedProvider = "Stripe";
    private bool isDialogOpen = false;

    protected override void OnInitialized()
    {
        _model = new SubscribeViewModel
        {
            Tiers = SiteSettings.Settings?.Subscription?.Tiers ?? new()
        };
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
        // TODO: Remove Link Hardcoding
        var res = await PaymentClient.NewSubscription(
            new WebServices.Fragments.Authorization.Payment.GetNewDetailsRequest
            {
                Level = selectedTier?.AmountCents != null ? selectedTier.AmountCents : 0,
                PostalCode = zipCode,
                SuccessUrl = "http://localhost:5000/subscribe/success",
                CancelUrl = "http://localhost:5000/subscribe/cancel"
            }
        );

        // TODO: Support Other Providers
        if (selectedProvider == "Stripe" && res is not null && !string.IsNullOrEmpty(res.Stripe.PaymentLink))
        {
            Navigation.NavigateTo(res.Stripe.PaymentLink);
        }

        // TODO: Display Error Messages
    }
}
