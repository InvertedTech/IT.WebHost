using IT.WebServices.Clients.Payment;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages.Subscribe
{
    public partial class SubscribeSuccess
    {
        [SupplyParameterFromQuery(Name = "session_id")]
        public string? SessionId { get; set; }
        [SupplyParameterFromQuery(Name = "processor")]
        public string? Provider { get; set; }

        [Inject] private PaymentClient PaymentClient { get; set; } = null!;
        [Inject] private NavigationManager Navigation { get; set; } = null!;
    }
}
