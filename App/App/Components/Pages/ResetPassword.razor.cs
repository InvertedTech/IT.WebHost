using Core.Extensions;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;
using ProtoValidate;

namespace WebApp.Components.Pages
{
    public partial class ResetPassword
    {
        [Inject] public UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IValidator Validator { get; set; } = null!;

        private string? newPassword { get; set; } = string.Empty;
        private string? confirmNewPassword { get; set; } = string.Empty;
        [SupplyParameterFromQuery(Name = "token")] public string? Token { get; set; } = string.Empty;

        private bool isLoading { get; set; } = false;
        private bool isSuccess { get; set; } = false;
        private bool isCheckingToken { get; set; } = true;
        private bool isTokenValid { get; set; } = false;
        private string? errorMessage { get; set; } = null;
        private IReadOnlyList<string> newPasswordErrors = [];

        protected override async Task OnInitializedAsync()
        {
            var res = await UserClient.ValidatePasswordResetTokenAsync(new ValidatePasswordResetTokenRequest
            {
                Token = this.Token,
            });

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                isTokenValid = true;
            }
            else
            {
                isTokenValid = false;
                errorMessage = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : "Reset link is invalid or has expired.";
            }

            isCheckingToken = false;
        }

        private void ClearValidation()
        {
            newPasswordErrors = [];
            errorMessage = null;
        }

        private async Task OnResetPasswordAsync()
        {
            errorMessage = null;
            newPasswordErrors = [];

            if (string.IsNullOrEmpty(newPassword) || newPassword != confirmNewPassword)
            {
                errorMessage = "Passwords do not match.";
                StateHasChanged();
                return;
            }

            var req = new CompleteForgotPasswordRequest
            {
                Token = Token,
                NewPassword = newPassword,
            };

            var validation = Validator.Validate(req, failFast: false);
            newPasswordErrors = validation.Violations.ForField("NewPassword").Errors;

            if (!validation.IsSuccess)
            {
                StateHasChanged();
                return;
            }

            isLoading = true;
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
