using Core.Extensions;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;
using ProtoValidate;

namespace WebApp.Components.Pages
{
    public partial class Signup
    {
        [Inject] public ONUserHelper userHelper { get; set; } = null!;
        [Inject] public UserInterface.UserInterfaceClient userClient { get; set; } = null!;
        [Inject] public NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IValidator Validator { get; set; } = null!;

        private CreateUserRequest signupRequest { get; set; } = new();
        private bool isSubmitting { get; set; } = false;
        private string? errorMessage { get; set; } = null;
        private IReadOnlyList<string> firstNameErrors = [];
        private IReadOnlyList<string> lastNameErrors = [];
        private IReadOnlyList<string> userNameErrors = [];
        private IReadOnlyList<string> emailErrors = [];
        private IReadOnlyList<string> passwordErrors = [];
        private IReadOnlyList<string> postalCodeErrors = [];
        private bool useUsernameAsDisplay { get; set; } = false;

        protected override void OnInitialized()
        {
            signupRequest = new CreateUserRequest();
        }

        private void ClearValidation()
        {
            firstNameErrors = [];
            lastNameErrors = [];
            userNameErrors = [];
            emailErrors = [];
            passwordErrors = [];
            postalCodeErrors = [];
            errorMessage = null;

            if (useUsernameAsDisplay)
            {
                signupRequest.DisplayName = signupRequest.UserName;
            }
        }

        private void OnUseUsernameToggled()
        {
            if (useUsernameAsDisplay)
            {
                signupRequest.DisplayName = signupRequest.UserName;
            }
        }

        private async Task OnSignupAsync()
        {
            isSubmitting = true;
            errorMessage = null;
            var validation = Validator.Validate(signupRequest, failFast: false);
            firstNameErrors = validation.Violations.ForField("FirstName").Errors;
            lastNameErrors = validation.Violations.ForField("LastName").Errors;
            userNameErrors = validation.Violations.ForField("UserName").Errors;
            emailErrors = validation.Violations.ForField("Email").Errors;
            passwordErrors = validation.Violations.ForField("Password").Errors;
            postalCodeErrors = validation.Violations.ForField("PostalCode").Errors;

            if (!validation.IsSuccess)
            {
                isSubmitting = false;
                StateHasChanged();
                return;
            }

            var res = await userClient.CreateUserAsync(signupRequest, userHelper.GetGrpcCallOptions());
            if (!string.IsNullOrEmpty(res.BearerToken))
            {
                isSubmitting = false;
                navigationManager.NavigateTo($"/auth/set-cookie?token={Uri.EscapeDataString(res.BearerToken)}&returnUrl=/", forceLoad: true);
            }
            else
            {
                errorMessage = res.Error.Message ?? "Error During Signup";
                isSubmitting = false;
                StateHasChanged();
            }
        }
    }
}
