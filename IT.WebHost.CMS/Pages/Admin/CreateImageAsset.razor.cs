using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class CreateImageAsset
    {
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private AssetInterface.AssetInterfaceClient AssetClient { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        private bool IsProcessing { get; set; } = false;

        private CreateAssetRequest createAssetRequest { get; set; } = new()
        {
            Image = new()
            {
                Public = new(),
                Private = new()
            }
        };

        // TODO: Add Error Responses
        public async Task OnSubmit()
        {
            IsProcessing = true;
            var res = await AssetClient.CreateAssetAsync(createAssetRequest, UserHelper.GetGrpcCallOptions());
            if (res.Record != null)
            {
                NavigationManager.NavigateTo("/admin/assets", true);
            } else
            {
                Console.WriteLine($"Upload Image Error: {res.Error?.Message}");
            }
            IsProcessing = false;
        }
    }
}
