using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Users
{
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private IEnumerable<UserSearchRecord> users = [];

    [Parameter, SupplyParameterFromQuery(Name = "pageSize")]
    public string PageSize { get; set; } = "20";

    [Parameter, SupplyParameterFromQuery(Name = "pageOffset")]
    public string PageOffset { get; set; } = "0";

    [Parameter, SupplyParameterFromQuery(Name = "q")]
    public string? Query { get; set; }

    private uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 20;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
    private uint PageTotalItems { get; set; }
    private int TotalPages => PageSizeParsed > 0 ? (int)Math.Ceiling((double)PageTotalItems / PageSizeParsed) : 1;
    private int CurrentPage => PageSizeParsed > 0 ? (int)(PageOffsetParsed / PageSizeParsed) + 1 : 1;

    private string _searchString = "";
    private bool _includeDeleted = false;
    private bool _isLoading;

    protected override async Task OnParametersSetAsync()
    {
        _searchString = Query ?? "";
        await LoadUsersAsync();
    }

    private void HandleSearchKey(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            NavigateFilter("q", string.IsNullOrWhiteSpace(_searchString) ? null : _searchString.Trim());
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

    private void NavigateFilter(string key, string? value)
    {
        Nav.NavigateTo(Nav.GetUriWithQueryParameters(new Dictionary<string, object?>
        {
            [key] = (object?)value,
            ["pageOffset"] = "0",
        }));
    }

    private async Task LoadUsersAsync()
    {
        _isLoading = true;
        try
        {
            var res = await UserClient.SearchUsersAdminAsync(new SearchUsersAdminRequest
            {
                PageSize = PageSizeParsed,
                PageOffset = PageOffsetParsed,
                SearchString = Query ?? string.Empty,
                IncludeDeleted = _includeDeleted,
            }, UserHelper.GetGrpcCallOptions());

            users = res.Records;
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
