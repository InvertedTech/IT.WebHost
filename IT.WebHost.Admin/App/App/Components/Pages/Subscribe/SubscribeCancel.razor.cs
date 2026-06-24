using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages.Subscribe
{
    public partial class SubscribeCancel
    {
        [SupplyParameterFromQuery(Name = "error")]
        public string? Error { get; set; }
    }
}
