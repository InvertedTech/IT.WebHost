using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Users
{
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private IEnumerable<UserRecord> users = new List<UserRecord>();

    private Task LoadUsersAsync(string? pageSize = null, string? pageOffset = null) => throw new NotImplementedException();
}
