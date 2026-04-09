using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Content
{
    [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private IEnumerable<ContentListRecord> ContentRecords = new List<ContentListRecord>();
    public string PageSize { get; set; } = "20";
    public string PageOffset { get; set; } = "0";
    private uint PageSizeParsesd => uint.TryParse(PageSize, out var v) ? v : 20;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 20;
    private uint PageTotalItems { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await LoadContentAsync();
    }
    private async Task LoadContentAsync()
    {
        try
        {
            var res = await ContentClient.GetAllContentAdminAsync(new GetAllContentAdminRequest
            {
                PageSize = PageSizeParsesd,
                PageOffset = PageOffsetParsed,
            }, UserHelper.GetGrpcCallOptions());

            if (res.Records.Count > 0)
            {
                ContentRecords = res.Records;
                PageTotalItems = res.PageTotalItems;
            }
        } catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }
}
