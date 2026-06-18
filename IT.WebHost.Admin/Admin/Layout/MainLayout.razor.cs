using Core.Extensions;
using IT.Web.Project1.Services;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using ProtoValidate;
using System.Security.Claims;

namespace Admin.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        private bool _disposed;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
        [Inject] private UserInterface.UserInterfaceClient users { get; set; } = null!;
        [Inject] private NavigationManager Nav { get; set; } = null!;
        [Inject] private IJSRuntime JS { get; set; } = null!;
        [Inject] private ProtectedLocalStorage Storage { get; set; } = null!;
        [Inject] private SiteSettingsService SiteSettings { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private IValidator Validator { get; set; } = null!;

        private string UserDisplayName =>
            !string.IsNullOrWhiteSpace(UserHelper.MyUser?.DisplayName)
                ? UserHelper.MyUser.DisplayName
                : !string.IsNullOrWhiteSpace(UserHelper.MyUser?.UserName)
                    ? UserHelper.MyUser.UserName
                    : "User";

        private string UserSubtitle
        {
            get
            {
                var email = GetUserEmail();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    return email;
                }

                return !string.IsNullOrWhiteSpace(UserHelper.MyUser?.UserName)
                    ? $"@{UserHelper.MyUser.UserName}"
                    : string.Empty;
            }
        }

        private string UserInitials => GetInitials(UserDisplayName);

        // Login State
        private string usernameOrEmail { get; set; } = string.Empty;
        private string password { get; set; } = string.Empty;
        private bool isSubmitting { get; set; } = false;
        private string? errorMessage { get; set; } = null;
        private IReadOnlyList<string> userNameErrors = [];
        private IReadOnlyList<string> passwordErrors = [];

        public readonly List<AdminNavItem> navItems = new()
        {
            new() { Text = "Dashboard", Href = "/",         Icon = "layout-dashboard" },
            new() { Text = "Content",   Href = "/content", Icon = "file-text", RequiredRole = RoleAbilities.ROLE_CAN_CREATE_CONTENT },
            new() { Text = "Assets",    Href = "/assets",  Icon = "image", RequiredRole = RoleAbilities.ROLE_CAN_CREATE_CONTENT },
            new() { Text = "Users",     Href = "/users",   Icon = "users", RequiredRole = $"{RoleAbilities.ROLE_MEMBER_MANAGER},{RoleAbilities.ROLE_IS_SUBSCRIPTION_MANAGER_OR_HIGHER}" },
        };

        private readonly List<AdminNavItem> settingsNavItems = new()
        {
            new() { Text = "CMS",             Href = "/settings/cms",             Icon = "file", RequiredRole = RoleAbilities.ROLE_IS_ADMIN_OR_OWNER },
            new() { Text = "Merch",           Href = "/settings/merch",           Icon = "shopping-bag", RequiredRole = RoleAbilities.ROLE_IS_ADMIN_OR_OWNER },
            new() { Text = "Personalization", Href = "/settings/personalization", Icon = "palette", RequiredRole = RoleAbilities.ROLE_IS_ADMIN_OR_OWNER },
            new() { Text = "Subscription",    Href = "/settings/subscription",    Icon = "credit-card", RequiredRole = RoleAbilities.ROLE_IS_ADMIN_OR_OWNER },
            new() { Text = "Comments",        Href = "/settings/comments",        Icon = "message-circle", RequiredRole = RoleAbilities.ROLE_IS_ADMIN_OR_OWNER },
            new() { Text = "Notifications",   Href = "/settings/notifications",   Icon = "bell", RequiredRole = RoleAbilities.ROLE_IS_ADMIN_OR_OWNER },
        };

        protected override async Task OnInitializedAsync()
        {
            await SiteSettings.LoadAsync();
            Nav.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (!_disposed) InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            _disposed = true;
            Nav.LocationChanged -= OnLocationChanged;
        }

        private async Task OnSubmitLogin()
        {
            isSubmitting = true;
            errorMessage = null;

            var request = new AuthenticateUserRequest
            {
                UserName = usernameOrEmail,
                Password = password,
                MFACode = string.Empty,
            };

            var validation = Validator.Validate(request, failFast: false);
            userNameErrors = validation.Violations.ForField("UserName").Errors;
            passwordErrors = validation.Violations.ForField("Password").Errors;

            if (!validation.IsSuccess)
            {
                isSubmitting = false;
                StateHasChanged();
                return;
            }

            var res = await users.AuthenticateUserAsync(request);

            if (!string.IsNullOrEmpty(res.BearerToken))
            {
                isSubmitting = false;
                Nav.NavigateTo($"/auth/set-cookie?token={Uri.EscapeDataString(res.BearerToken)}&returnUrl=/", forceLoad: true);
            }
            else
            {
                errorMessage = res.Error.Message ?? "Error Logging In";
                isSubmitting = false;
                StateHasChanged();
            }
        }

        private void ClearLoginValidation()
        {
            userNameErrors = [];
            passwordErrors = [];
            errorMessage = null;
        }

        private string? GetUserEmail()
        {
            var claims = UserHelper.MyUser?.ExtraClaims;
            if (claims is null || claims.Count == 0)
            {
                return null;
            }

            var emailClaim = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email
                || c.Type.Equals("email", StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrWhiteSpace(emailClaim?.Value) ? null : emailClaim.Value;
        }

        private static string GetInitials(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return "?";
            }

            var parts = displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}"
                : displayName[..Math.Min(2, displayName.Length)].ToUpper();
        }
    }

    public record AdminNavItem
    {
        public string Text { get; init; } = "";
        public string Href { get; init; } = "";
        public string Icon { get; init; } = "";
        public string? RequiredRole { get; init; } = null;
    }
}
