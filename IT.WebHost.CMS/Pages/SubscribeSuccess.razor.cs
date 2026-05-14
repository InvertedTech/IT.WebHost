using IT.WebServices.Clients.Payment;
using IT.WebServices.Fragments.Authorization.Payment.Stripe;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages
{
    public partial class SubscribeSuccess
    {
        [SupplyParameterFromQuery(Name = "session_id")]
        public string? SessionId { get; set; }
        [SupplyParameterFromQuery(Name = "processor")]
        public string? Provider { get; set; }
        [Inject] private PaymentClient PaymentClient { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;

        // TODO: Calls Twice, Calls FinishStripe Twice, makes more subs than should be made
        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(Provider))
                Navigation.NavigateTo($"/subscribe/cancel?error={"provider is null"}");

            if (string.IsNullOrEmpty(SessionId))
                Navigation.NavigateTo($"/subscribe/cancel?error={"session id is null"}");

            if (Provider == "stripe")
                await FinishStripe(SessionId);
        }

        private async Task FinishStripe(string sessionId)
        {
            var req = new StripeFinishOwnSubscriptionRequest
            {
                ProcessorSessionID = sessionId
            };

            var res = await PaymentClient.FinishStripe(req);
            if (res is null)
            {
                // TODO: Navigate to /subscribe/cancel?error={"No Response"}
                return;
            }
            if (res is not null && !string.IsNullOrEmpty(res.Error))
            {
                // TODO: Navigate to /subscribe/cancel?error={res.Error}
                return;
            }

            Navigation.NavigateTo("/profile");
        }
    }
}
