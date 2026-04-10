using BlazorBlueprint.Components;
using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class MembersArea
{
    [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;
    [Inject] private SiteSettingsService SiteSettings { get; set; } = null!;

    private IEnumerable<ContentListRecord> ContentRecords { get; set; } = [];
    private LayoutEnum Layout { get; set; } = LayoutEnum.List;
    private uint NextPageOffset { get; set; } = 0;
    private uint PageSize { get; set; } = 10;
    private uint TotalItems { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Layout = SiteSettings.Settings?.CMS?.DefaultLayout ?? LayoutEnum.List;

        var res = await ContentClient.GetAllContentAsync(new GetAllContentRequest
        {
            PageSize = PageSize,
            PageOffset = 0,
            SubscriptionSearch = new SubscriptionLevelSearch
            {
                MinimumLevel = 10,
                MaximumLevel = 9999,
            }
        }, UserHelper.GetGrpcCallOptions());

        ContentRecords = res.Records.ToList();
        NextPageOffset = res.PageOffsetEnd;
        TotalItems = res.PageTotalItems;
    }
}
