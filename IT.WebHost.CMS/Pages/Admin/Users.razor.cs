using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Users
{
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;
    [Inject] private NavigationManager Nav { get; set; } = null!;

    private IEnumerable<UserNormalRecord> users = new List<UserNormalRecord>();

    [Parameter, SupplyParameterFromQuery(Name = "pageSize")]
    public string PageSize { get; set; } = "20";

    [Parameter, SupplyParameterFromQuery(Name = "pageOffset")]
    public string PageOffset { get; set; } = "0";

    private uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 20;
    private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
    private uint PageTotalItems { get; set; }
    private int TotalPages => PageSizeParsed > 0 ? (int)Math.Ceiling((double)PageTotalItems / PageSizeParsed) : 1;
    private int CurrentPage => PageSizeParsed > 0 ? (int)(PageOffsetParsed / PageSizeParsed) + 1 : 1;
    private bool _isLoading;

    protected override async Task OnParametersSetAsync()
    {
        await LoadUsersAsync();
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

    private async Task LoadUsersAsync()
    {
        _isLoading = true;
        try
        {
            var res = await UserClient.GetAllUsersAsync(new GetAllUsersRequest
            {
                PageSize = PageSizeParsed,
                PageOffset = PageOffsetParsed,
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
