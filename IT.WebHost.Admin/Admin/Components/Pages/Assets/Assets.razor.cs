using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Assets
{
    public partial class Assets
    {
        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        public List<AssetListRecord> AssetList { get; private set; } = new List<AssetListRecord>();
        [Inject] private AssetClient AssetClient { get; set; } = null!;
        [Inject] private NavigationManager Nav { get; set; } = null!;
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
        private uint totalItems { get; set; } = 0;

        protected override async Task OnParametersSetAsync()
        {
            // TODO: This page calls SearchAsset requiring ROLE_CAN_CREATE_CONTENT_OR_SERVICE ("con_publisher,con_writer,admin,owner,service"); wrap UI with AuthorizeView
            await LoadAssets();
            StateHasChanged();
        }

        private async Task LoadAssets()
        {
            var req = new SearchAssetRequest()
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset,
            };

            var res = await AssetClient.SearchAsset(req);

            if (res is null)
            {
                throw new Exception("No Response");
            }

            AssetList = res.Records.ToList();
            totalItems = res.PageTotalItems;
        }
    }
}
