using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authorization.Payment.Fortis;
using IT.WebServices.Fragments.Authorization.Payment.Stripe;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("subscribe")]
    public class SubscribeController : Controller
    {
        private readonly FortisInterface.FortisInterfaceClient fortisClient;
        private readonly StripeInterface.StripeInterfaceClient stripeClient;
        private readonly ONUserHelper userHelper;

        public SubscribeController(StripeInterface.StripeInterfaceClient stripeClient, ONUserHelper userHelper, FortisInterface.FortisInterfaceClient fortisClient)
        {
            this.stripeClient = stripeClient;
            this.userHelper = userHelper;
            this.fortisClient = fortisClient;
        }

        [HttpGet("success")]
        public async Task<IActionResult> Success(string session_id, string processor)
        {
            if (string.IsNullOrWhiteSpace(session_id))
                return Redirect("/subscribe");

            if (processor == "fortis")
                await fortisClient.FortisFinishOwnSubscriptionAsync(new() { TransactionID = session_id }, userHelper.GetGrpcCallOptions());

            if (processor == "stripe")
                await stripeClient.StripeFinishOwnSubscriptionAsync(new() { ProcessorSessionID = session_id }, userHelper.GetGrpcCallOptions());

            return Redirect("/auth/refreshtoken?url=/subscribe/");
        }
    }
}
