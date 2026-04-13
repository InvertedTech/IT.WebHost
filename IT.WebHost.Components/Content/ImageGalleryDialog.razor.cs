using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.Components.Content
{
    public partial class ImageGalleryDialog
    {
        [Inject] private AssetInterface.AssetInterfaceClient AssetClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;

        [Parameter] public bool IsDialogOpen { get; set; } = false;
        [Parameter] public EventCallback<bool> IsDialogOpenChanged { get; set; }

        [Parameter] public string SelectedAssetId { get; set; } = string.Empty;
        [Parameter] public EventCallback<string> SelectedAssetIdChanged { get; set; }

        [Parameter] public IReadOnlyList<string> SelectedAssetIds { get; set; } = [];
        [Parameter] public EventCallback<IReadOnlyList<string>> SelectedAssetIdsChanged { get; set; }

        [Parameter] public bool MultiSelect { get; set; } = false;

        private IEnumerable<AssetListRecord> AssetRecords { get; set; } = [];
        private uint PageSize { get; set; } = 20;
        private uint PageOffset { get; set; } = 0;

        private string _pendingSingleId = string.Empty;
        private HashSet<string> _pendingMultiIds = [];

        protected override void OnParametersSet()
        {
            _pendingSingleId = SelectedAssetId;
            _pendingMultiIds = [.. SelectedAssetIds];
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadAssets();
        }

        private async Task LoadAssets()
        {
            var request = new GetImageAssetsRequest
            {
                PageSize = PageSize,
                PageOffset = PageOffset
            };
            var response = await AssetClient.GetImageAssetsAsync(request, UserHelper.GetGrpcCallOptions());
            AssetRecords = response.Records;
            StateHasChanged();
        }

        private void ToggleSelection(string? assetId)
        {
            if (string.IsNullOrEmpty(assetId)) return;

            if (MultiSelect)
            {
                if (!_pendingMultiIds.Add(assetId))
                    _pendingMultiIds.Remove(assetId);
            }
            else
            {
                _pendingSingleId = _pendingSingleId == assetId ? string.Empty : assetId;
            }
        }

        private bool IsAssetSelected(string assetId) =>
            MultiSelect ? _pendingMultiIds.Contains(assetId) : _pendingSingleId == assetId;

        private async Task Confirm()
        {
            if (MultiSelect)
                await SelectedAssetIdsChanged.InvokeAsync([.. _pendingMultiIds]);
            else
                await SelectedAssetIdChanged.InvokeAsync(_pendingSingleId);

            await IsDialogOpenChanged.InvokeAsync(false);
            IsDialogOpen = false;
        }
    }
}
