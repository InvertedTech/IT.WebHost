using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Login
{
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
    [Parameter]
    [SupplyParameterFromQuery(Name = "redirect")]
    public string Redirect { get; set; } = "/";


    private string? email;
    private string? password;
    private string? errorMessage;

    private async Task HandleLogin()
    {
        var res = await UserClient.AuthenticateUserAsync(new AuthenticateUserRequest()
        {
            UserName = email,
            Password = password,
            MFACode = ""
        });

        if (!string.IsNullOrEmpty(res.BearerToken))
        {
            var returnUrl = Uri.EscapeDataString(Redirect ?? "/");
            Nav.NavigateTo($"/auth/set-cookie?token={Uri.EscapeDataString(res.BearerToken)}&returnUrl={returnUrl}", forceLoad: true);
        } else
        {
            errorMessage = res.Error.Message ?? "Error Logging In";
            StateHasChanged();
        }

        return;
    }
}
