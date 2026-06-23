using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class Home
    {
        [Inject] public ContentClient contentClient { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        private int pageSize => int.Parse(PageSizeStr ?? "25");

        [SupplyParameterFromQuery(Name = "offset")]
        public string? PageOffsetStr { get; set; }
        private int pageOffset => int.Parse(PageOffsetStr ?? "0");

        private IEnumerable<ContentListRecord> content { get; set; } = [];
        private uint pageOffsetEnd;
        private uint pageTotalItems;

        protected override async Task OnParametersSetAsync()
        {
            await LoadContent();
        }

        private async Task LoadContent()
        {
            var req = new SearchContentRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset
            };

            var res = await contentClient.Search(req);

            content = res.Records;
            pageOffsetEnd = res.PageOffsetEnd;
            pageTotalItems = res.PageTotalItems;
        }
    }
}
