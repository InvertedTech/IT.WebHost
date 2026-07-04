using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace WebApp.Components.Pages
{
    public partial class ForgotPassword
    {
        [Inject] public UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
        [Inject] public ONUserHelper UserHelper { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        private string? email;
        private bool isLoading { get; set; } = false;
        private bool showCompletedMessage { get; set; } = false;

        protected override void OnInitialized()
        {
            isLoading = false;
            showCompletedMessage = false;
            email = null;
            base.OnInitialized();
        }

        private async Task Submit()
        {
            isLoading = true;

            var req = new StartForgotPasswordRequest
            {
                Email = email,
            };

            var res = await UserClient.StartForgotPasswordAsync(req);
            isLoading = false;
            showCompletedMessage = true;
            StateHasChanged();
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}
