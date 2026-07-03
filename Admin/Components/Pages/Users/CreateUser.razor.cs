using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Users
{
    public partial class CreateUser
    {
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public UserInterface.UserInterfaceClient UsersClient { get; set; } = null!;
        [Inject] public ONUserHelper UserHelper { get; set; } = null!;

        private string? username { get; set; }
        private string? displayName { get; set; }
        private string? bio { get; set; }
        private string? email { get; set; }
        private string? password { get; set; }
        private string? firstName { get; set; }
        private string? lastName { get; set; }
        private string? postalCode { get; set; }
        private List<string>? selectedRoles { get; set; }

        protected override void OnInitialized()
        {
            username = string.Empty;
            displayName = string.Empty;
            email = string.Empty;
            password = string.Empty;
            firstName = string.Empty;
            lastName = string.Empty;
            postalCode = string.Empty;
            selectedRoles = new List<string>();
        }

        private async Task SubmitCreate()
        {
            var req = new AdminCreateUserRequest
            {
                UserName = username,
                Password = password,
                DisplayName = displayName,
                Bio = bio,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PostalCode = postalCode
            };

            if (selectedRoles is not null && selectedRoles.Any())
                req.Roles.AddRange(selectedRoles);
        }
    }
}
