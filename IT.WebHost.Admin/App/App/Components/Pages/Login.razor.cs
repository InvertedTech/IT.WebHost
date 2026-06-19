using Core.Extensions;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;
using ProtoValidate;

namespace WebApp.Components.Pages
{
    public partial class Login
    {
        [Inject] public ONUserHelper userHelper { get; set; } = null!;
        [Inject] public UserInterface.UserInterfaceClient userClient { get; set; } = null!;
        [Inject] public NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IValidator Validator { get; set; } = null!;

        private AuthenticateUserRequest loginRequest { get; set; } = new();
        private bool isSubmitting { get; set; } = false;
        private string? errorMessage { get; set; } = null;
        private IReadOnlyList<string> userNameErrors = [];
        private IReadOnlyList<string> passwordErrors = [];

        protected override void OnInitialized()
        {
            loginRequest = new AuthenticateUserRequest();
        }

        private void ClearValidation()
        {
            userNameErrors = [];
            passwordErrors = [];
            errorMessage = null;
        }

        private async Task OnLoginAsync()
        {
            isSubmitting = true;
            errorMessage = null;
            var validation = Validator.Validate(loginRequest, failFast: false);
            userNameErrors = validation.Violations.ForField("UserName").Errors;
            passwordErrors = validation.Violations.ForField("Password").Errors;

            if (!validation.IsSuccess)
            {
                isSubmitting = false;
                StateHasChanged();
                return;
            }

            var res = await userClient.AuthenticateUserAsync(loginRequest, userHelper.GetGrpcCallOptions());
            if (!string.IsNullOrEmpty(res.BearerToken))
            {
                isSubmitting = false;
                navigationManager.NavigateTo($"/auth/set-cookie?token={Uri.EscapeDataString(res.BearerToken)}&returnUrl=/", forceLoad: true);
            }
            else
            {
                errorMessage = res.Error.Message ?? "Error Logging In";
                isSubmitting = false;
                StateHasChanged();
            }
        }
    }
}
