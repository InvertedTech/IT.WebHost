using IT.WebServices.Fragments.Authorization;
using Google.Protobuf.Collections;

namespace IT.WebHost.Core.Models
{
    public class SubscribeViewModel
    {
        public RepeatedField<SubscriptionTier> Tiers { get; set; } = new RepeatedField<SubscriptionTier>();
    }
}
