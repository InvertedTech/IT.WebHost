using IT.WebServices.Fragments.Authorization;
using Google.Protobuf.Collections;

namespace IT.WebHost.App.Models
{
    public class SubscribeViewModel
    {
        public RepeatedField<SubscriptionTier> Tiers { get; set; } = new RepeatedField<SubscriptionTier>();
    }
}