using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class Signup
{
    [Inject] private NavigationManager Nav { get; set; } = null!;
    [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;

    private string? userName;
    private string? email;
    private string? password;
    private string? confirmPassword;
    private string? displayName;
    private string? bio;
    private string? firstName;
    private string? lastName;
    private string? postalCode;
    private string? errorMessage;

    private async Task HandleComplete()
    {
        try
        {
            var req = new CreateUserRequest
            {
                UserName = userName,
                Email = email,
                DisplayName = displayName,
                Password = password,
                Bio = bio,
                FirstName = firstName,
                LastName  = lastName,
                PostalCode = postalCode,
            };

            var res = await UserClient.CreateUserAsync(req);
            if (res == null)
            {
                errorMessage = "No Response From Server, Try Again Later";
                StateHasChanged();
                return;
            }

            if (res.Error is not null && res.Error.Reason != APIErrorReason.ErrorReasonNoError)
            {
                errorMessage = res.Error.Message;
                StateHasChanged();
                return;
            }


            Nav.NavigateTo($"/auth/set-cookie?token={Uri.EscapeDataString(res.BearerToken)}&returnUrl={"/profile"}", forceLoad: true);
            return;
        } catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            this.errorMessage = ex.Message;

            StateHasChanged();
            return;
        }
    }
}
