using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages
{
    public partial class SubscribeCancel
    {
        [SupplyParameterFromQuery(Name = "error")]
        public string? Error { get; set; }
    }
}
