using System.Reflection;
using BlazorBlueprint.Components;
using BlazorBlueprint.Primitives.Services;
using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Index
{
    [Inject] private ContentClient ContentClient { get; set; } = null!;

    private IEnumerable<ContentListRecord> ContentRecords { get; set; } = new List<ContentListRecord>();
    private LayoutEnum Layout { get; set; } = LayoutEnum.List;
    private uint NextPageOffset { get; set; } = 0;
    private uint PageSize { get; set; } = 10;
    private uint TotalItems { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadInitialContent();
    }

    private async Task LoadInitialContent()
    {
        var res = await ContentClient.GetAll(new GetAllContentRequest
        {
            PageSize = PageSize,
            PageOffset = 0,
        });


        ContentRecords = res.Records.ToList();
        NextPageOffset = res.PageOffsetEnd;
        TotalItems = res.PageTotalItems;
    }
}
