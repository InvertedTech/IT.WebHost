using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Assets
{
    [Inject] private AssetInterface.AssetInterfaceClient AssetClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    public IEnumerable<AssetListRecord> AssetRecords { get; set; } = [];

    [Parameter, SupplyParameterFromQuery(Name = "pageSize")]
    public string PageSize { get; set; } = "20";

    [Parameter, SupplyParameterFromQuery(Name = "pageOffset")]
    public string PageOffset { get; set; } = "0";

    private uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 20;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
    private uint PageTotalItems { get; set; }

    private string BaseUrl => "/admin/assets";

    protected override async Task OnParametersSetAsync()
    {
        await LoadAssetsAsync();
    }

    private async Task LoadAssetsAsync()
    {
        var res = await AssetClient.GetImageAssetsAsync(new GetImageAssetsRequest
        {
            PageSize = PageSizeParsed,
            PageOffset = PageOffsetParsed,
        }, UserHelper.GetGrpcCallOptions());

        AssetRecords = res.Records;
        PageTotalItems = res.PageTotalItems;
    }
}
