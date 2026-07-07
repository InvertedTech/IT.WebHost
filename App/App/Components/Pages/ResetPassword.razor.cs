using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class ResetPassword
    {
        [Inject] public UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;

        private string? newPassword { get; set; } = string.Empty;
        private string? confirmNewPassword { get; set; } = string.Empty;
        [SupplyParameterFromQuery(Name = "token")] public string? Token { get; set; } = string.Empty;

        private bool isLoading { get; set; } = false;
        private bool isSuccess { get; set; } = false;
        private string? errorMessage { get; set; } = null;

        private async Task OnResetPasswordAsync()
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(newPassword) || newPassword != confirmNewPassword)
            {
                errorMessage = "Passwords do not match.";
                StateHasChanged();
                return;
            }

            isLoading = true;
            var req = new CompleteForgotPasswordRequest
            {
                Token = Token,
                NewPassword = newPassword,
            };

            var res = await UserClient.CompleteForgotPasswordAsync(req);
            isLoading = false;

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                isSuccess = true;
            }
            else
            {
                errorMessage = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : "Failed to reset password.";
            }

            StateHasChanged();
        }

        private void GoToLogin()
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}
