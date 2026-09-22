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
            get => int.Parse(PageSizeStr ?? "10");
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
        private string? errorMessage { get; set; }
        private uint totalItems { get; set; } = 0;
        private List<GenericMerchRecord> merch { get; set; } = new List<GenericMerchRecord>();

        protected override async Task OnParametersSetAsync()
        {
            await LoadMerch();
        }

        private async Task LoadMerch()
        {
            isLoading = true;
            errorMessage = null;
            var req = BuildRequest();
            var res = await merchClient.Search(req);
            if (res is not null)
            {
                merch = res.Records.ToList();
                totalItems = res.PageTotalItems;
            }
            else
            {
                merch = [];
                totalItems = 0;
                errorMessage = "We couldn't load merch right now. Please try again.";
            }
            isLoading = false;
            StateHasChanged();
        }

        private SearchMerchRequest BuildRequest()
        {
            var req = new SearchMerchRequest
            {
                PageOffset = (uint)pageOffset,
                PageSize = (uint)pageSize,
            };
            if (!string.IsNullOrEmpty(Query))
            {
                req.Query = Query;
            }
            return req;
        }
    }
}
