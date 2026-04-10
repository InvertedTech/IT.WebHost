using IT.WebServices.Fragments.Merch;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Merch
{
    [Inject] private MerchInterface.MerchInterfaceClient MerchClient { get; set; } = null!;

    [SupplyParameterFromQuery(Name = "q")] public string? Query { get; set; }
    [SupplyParameterFromQuery(Name = "pageSize")] public string? PageSize { get; set; }
    [SupplyParameterFromQuery(Name = "pageOffset")] public string? PageOffset { get; set; }
    [SupplyParameterFromQuery(Name = "tag")] public string? Tag { get; set; }
    [SupplyParameterFromQuery(Name = "onlyInStock")] public string? OnlyInStock { get; set; }

    private IEnumerable<GenericMerchRecord> Records { get; set; } = [];
    private uint PageTotalItems { get; set; }

    private uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 10;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
    private bool OnlyInStockParsed => bool.TryParse(OnlyInStock, out var v) && v;

    private string BaseMerchUrl
    {
        get
        {
            var qs = $"q={Uri.EscapeDataString(Query ?? string.Empty)}";
            if (!string.IsNullOrEmpty(Tag))
                qs += $"&tag={Uri.EscapeDataString(Tag)}";
            if (OnlyInStockParsed)
                qs += "&onlyInStock=true";
            return $"/merch?{qs}";
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        var res = await MerchClient.SearchMerchAsync(new SearchMerchRequest
        {
            Query = Query ?? string.Empty,
            PageSize = PageSizeParsed,
            PageOffset = PageOffsetParsed,
            Tag = Tag ?? string.Empty,
            OnlyInStock = OnlyInStockParsed
        });

        Records = res.Records.ToList();
        PageTotalItems = res.PageTotalItems;
    }
}
