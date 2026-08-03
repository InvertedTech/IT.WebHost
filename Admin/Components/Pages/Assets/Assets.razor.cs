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

        [SupplyParameterFromQuery(Name = "q")]
        public string? QueryFilter { get; set; }

        [SupplyParameterFromQuery(Name = "type")]
        public string? AssetTypeFilter { get; set; }

        private uint totalItems { get; set; } = 0;

        private string _queryFilter = string.Empty;
        private string? _assetTypeFilter;
        private bool _isCreateDialogOpen;

        private string ExtraQuery
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(QueryFilter))
                    parts.Add($"q={Uri.EscapeDataString(QueryFilter)}");
                if (!string.IsNullOrEmpty(AssetTypeFilter))
                    parts.Add($"type={Uri.EscapeDataString(AssetTypeFilter)}");
                return string.Join("&", parts);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            _queryFilter = QueryFilter ?? string.Empty;
            _assetTypeFilter = AssetTypeFilter;

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

            if (!string.IsNullOrEmpty(QueryFilter))
                req.Query = QueryFilter;

            if (!string.IsNullOrEmpty(AssetTypeFilter) && Enum.TryParse<AssetType>(AssetTypeFilter, out var assetType))
                req.AssetType = assetType;

            var res = await AssetClient.SearchAsset(req);

            if (res is null)
            {
                throw new Exception("No Response");
            }

            AssetList = res.Records.ToList();
            totalItems = res.PageTotalItems;
        }

        private void ApplyFilters(string query, string? assetType)
        {
            var parts = new List<string> { $"size={pageSize}" };
            if (!string.IsNullOrEmpty(query))
                parts.Add($"q={Uri.EscapeDataString(query)}");
            if (!string.IsNullOrEmpty(assetType))
                parts.Add($"type={Uri.EscapeDataString(assetType)}");

            Nav.NavigateTo($"/assets?{string.Join("&", parts)}");
        }

        private void ClearFilters() => Nav.NavigateTo("/assets");

        private async Task HandleAssetCreated(ImageAssetRecord asset)
        {
            _isCreateDialogOpen = false;
            await LoadAssets();
            StateHasChanged();
        }
    }
}
