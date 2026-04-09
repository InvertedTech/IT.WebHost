using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Assets
{
    [Inject] private AssetInterface.AssetInterfaceClient AssetClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    public IEnumerable<AssetListRecord> AssetRecords { get; set; } = new List<AssetListRecord>();
    public string PageSize { get; set; } = "20";
    public string PageOffset { get; set; } = "0";
    private uint PageSizeParsesd => uint.TryParse(PageSize, out var v) ? v : 20;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 20;
    private uint PageTotalItems { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadAssetsAsync();
    }

    // TODO: Make A BaseURL

    private async Task LoadAssetsAsync()
    {
        var res = await AssetClient.GetImageAssetsAsync(new GetImageAssetsRequest {
            PageSize = PageSizeParsesd,
            PageOffset = PageOffsetParsed,
        }, UserHelper.GetGrpcCallOptions());

        if (res.Records.Count > 0)
        {
            AssetRecords = res.Records;
            PageTotalItems = res.PageTotalItems;
        }
    }
    private Task SearchAssetsAsync(string query) => throw new NotImplementedException();
}
