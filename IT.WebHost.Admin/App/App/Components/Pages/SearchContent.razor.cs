using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class SearchContent
    {
        [Inject] private ContentClient ContentClient { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        public List<ContentListRecord> ContentList { get; private set; } = new List<ContentListRecord>();
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

        protected override async Task OnParametersSetAsync()
        {
            await GetContent();
        }

        private async Task GetContent()
        {
            isLoading = true;
            var req = BuildRequest();
            var res = await ContentClient.Search(req);
            if (res != null)
            {
                ContentList = res.Records.ToList();
                totalItems = res.PageTotalItems;
            }

            isLoading = false;
            StateHasChanged();
        }

        private SearchContentRequest BuildRequest()
        {
            var req = new SearchContentRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset
            };

            if (!string.IsNullOrEmpty(Query))
            {
                req.Query = Query;
            }

            return req;
        }
    }
}
