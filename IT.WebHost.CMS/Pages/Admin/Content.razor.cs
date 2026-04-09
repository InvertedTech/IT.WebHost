using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Content
{
    [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private SiteSettingsService SiteSettings { get; set; } = null!;

    private IEnumerable<ContentListRecord> ContentRecords = new List<ContentListRecord>();

    [Parameter, SupplyParameterFromQuery(Name = "pageSize")]
    public string PageSize { get; set; } = "20";

    [Parameter, SupplyParameterFromQuery(Name = "pageOffset")]
    public string PageOffset { get; set; } = "0";

    [Parameter, SupplyParameterFromQuery(Name = "channelId")]
    public string? ChannelId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "categoryId")]
    public string? CategoryId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "contentType")]
    public string? ContentTypeStr { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "query")]
    public string? Query { get; set; }

    private uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 20;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
    private uint PageTotalItems { get; set; }
    private int TotalPages => PageSizeParsed > 0 ? (int)Math.Ceiling((double)PageTotalItems / PageSizeParsed) : 1;
    private int CurrentPage => PageSizeParsed > 0 ? (int)(PageOffsetParsed / PageSizeParsed) + 1 : 1;
    private ContentType ContentTypeParsed => Enum.TryParse<ContentType>(ContentTypeStr, out var ct) ? ct : ContentType.ContentNone;

    private IEnumerable<ChannelRecord> Channels => SiteSettings.Settings?.CMS?.Channels?.AsEnumerable() ?? [];
    private IEnumerable<CategoryRecord> Categories => SiteSettings.Settings?.CMS?.Categories?.AsEnumerable() ?? [];

    private string _channelId = "";
    private string _categoryId = "";
    private ContentType _contentType = ContentType.ContentNone;
    private string _query = "";
    private bool _isLoading;

    private string BaseUrl => Nav.GetUriWithQueryParameters(new Dictionary<string, object?>
    {
        ["pageSize"] = (object?)null,
        ["pageOffset"] = (object?)null,
        ["channelId"] = (object?)null,
        ["categoryId"] = (object?)null,
        ["contentType"] = (object?)null,
        ["query"] = (object?)null,
    });

    protected override async Task OnParametersSetAsync()
    {
        _channelId = ChannelId ?? "";
        _categoryId = CategoryId ?? "";
        _contentType = ContentTypeParsed;
        _query = Query ?? "";

        await LoadContentAsync();
    }

    private void OnPageChanged(int page)
    {
        Nav.NavigateTo(Nav.GetUriWithQueryParameters(new Dictionary<string, object?>
        {
            ["pageOffset"] = ((page - 1) * (int)PageSizeParsed).ToString(),
        }));
    }

    private void OnPageSizeSelectChanged(uint size)
    {
        Nav.NavigateTo(Nav.GetUriWithQueryParameters(new Dictionary<string, object?>
        {
            ["pageSize"] = size.ToString(),
            ["pageOffset"] = "0",
        }));
    }

    private void OnChannelChanged(string val) =>
        NavigateFilter("channelId", string.IsNullOrEmpty(val) ? null : val);

    private void OnCategoryChanged(string val) =>
        NavigateFilter("categoryId", string.IsNullOrEmpty(val) ? null : val);

    private void OnContentTypeChanged(ContentType ct) =>
        NavigateFilter("contentType", ct == ContentType.ContentNone ? null : ct.ToString());

    private void OnQuerySearch(string val) =>
        NavigateFilter("query", string.IsNullOrWhiteSpace(val) ? null : val.Trim());

    private void NavigateFilter(string key, string? value)
    {
        Nav.NavigateTo(Nav.GetUriWithQueryParameters(new Dictionary<string, object?>
        {
            [key] = (object?)value,
            ["pageOffset"] = "0",
        }));
    }

    private async Task LoadContentAsync()
    {
        _isLoading = true;
        try
        {
            var req = new GetAllContentAdminRequest
            {
                PageSize = PageSizeParsed,
                PageOffset = PageOffsetParsed,
                ChannelId = ChannelId ?? string.Empty,
                CategoryId = CategoryId ?? string.Empty,
                Tag = Query ?? string.Empty,
                ContentType = ContentTypeParsed,
            };

            var res = await ContentClient.GetAllContentAdminAsync(req, UserHelper.GetGrpcCallOptions());
            ContentRecords = res.Records;
            PageTotalItems = res.PageTotalItems;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            _isLoading = false;
        }
    }
}
