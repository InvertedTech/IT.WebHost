using IT.WebServices.Clients.Merch;
using IT.WebServices.Fragments.Merch;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class Merch
    {
        [Inject] private MerchClient merchClient { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        private int pageSize
        {
            get => int.Parse(PageSizeStr ?? "25");
        }
        [SupplyParameterFromQuery(Name = "offset")]
        public string? PageOffsetStr { get; set; }
        private int pageOffset
        {
            get => int.Parse(PageOffsetStr ?? "0");
        }

        [SupplyParameterFromQuery(Name = "query")]
        public string? Query { get; set; }

        private bool isLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;
        private List<GenericMerchRecord> merch { get; set; } = new List<GenericMerchRecord>();

        protected override async Task OnParametersSetAsync()
        {
            await LoadMerch();
        }

        private async Task LoadMerch()
        {
            isLoading = true;
            var req = new SearchMerchRequest()
            {
                PageOffset = (uint)pageOffset,
                PageSize = (uint)pageSize,
            };

            if (!string.IsNullOrEmpty(Query))
            {
                req.Query = Query;
            }

            var res = await merchClient.SearchMerch(req);
            if (res is not null && res.Any())
            {
                merch = res.ToList();
            }
            isLoading = false;
            StateHasChanged();
        }
    }
}
