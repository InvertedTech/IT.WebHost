using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Login
{
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;

    private string? email;
    private string? password;
    private string? errorMessage;

    private Task HandleLogin() => throw new NotImplementedException();
}
