using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class Home
    {
        [Inject] public ContentClient contentCllient { get; set; } = null!;
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

        private IEnumerable<ContentListRecord> content { get; set; } = [];

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

            var res = await contentCllient.Search(req);

            if (res.PageTotalItems > 0)
            {
                content = res.Records;
                StateHasChanged();
            }
        }
    }
}
