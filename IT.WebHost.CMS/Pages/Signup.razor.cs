using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Signup
{
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;

    private string? email;
    private string? password;
    private string? displayName;
    private string? bio;
    private string? errorMessage;

    private Task HandleComplete() => throw new NotImplementedException();
}
