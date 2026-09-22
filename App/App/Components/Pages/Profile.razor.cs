using Core.Extensions;
using IT.WebServices.Authentication;
using IT.WebServices.Clients.Payment;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;
using ProtoValidate;

namespace WebApp.Components.Pages
{
    public partial class Profile
    {
        [Inject] public ONUserHelper userHelper { get; set; } = null!;
        [Inject] public UserInterface.UserInterfaceClient userClient { get; set; } = null!;
        [Inject] public PaymentClient paymentClient { get; set; } = null!;
        [Inject] public IToastService ToastService { get; set; } = null!;
        [Inject] private IValidator Validator { get; set; } = null!;

        private UserNormalRecord? user { get; set; }
        private IEnumerable<GenericSubscriptionFullRecord> SubscriptionRecords { get; set; } = [];
        private IEnumerable<TOTPDeviceLimited> TOTPDevices { get; set; } = [];
        private ProfileData? EditProfile { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsEditing { get; set; }
        private bool isResetPasswordOpen { get; set; } = false;
        private bool isSavingPassword { get; set; } = false;
        private string oldPassword { get; set; } = string.Empty;
        private string newPassword { get; set; } = string.Empty;
        private string confirmNewPassword { get; set; } = string.Empty;
        private IReadOnlyList<string> passwordErrors { get; set; } = [];
        private bool isAddTotpOpen { get; set; } = false;
        private int addTotpStep { get; set; } = 1;
        private string newTotpDeviceName { get; set; } = string.Empty;
        private string newTotpCode { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {

            IsLoading = true;
            IsEditing = false;
            user = null;
            await LoadUser();
            EditProfile = user is not null ? ProfileData.FromRecord(user) : null;
            IsLoading = false;
        }

        private async Task LoadUser()
        {
            await Task.WhenAll(GetUser(), GetUserSubs()/*, GetUserTOTP()*/);
        }

        private async Task GetUser()
        {
            var res = await userClient.GetOwnUserAsync(
                new GetOwnUserRequest(),
                userHelper.GetGrpcCallOptions());

            user = res?.Record;
        }

        private async Task GetUserSubs()
        {
            var req = new GetOwnSubscriptionRecordsRequest();
            var res = await paymentClient.GetOwnSubscriptions(req);

            if (res is not null)
            {
                SubscriptionRecords = res.Generic.ToList();
            }
        }

        private async Task GetUserTOTP()
        {
            var res = await userClient.GetOwnTotpListAsync(
                new GetOwnTotpListRequest(),
                userHelper.GetGrpcCallOptions());

            if (res is not null)
            {
                TOTPDevices = res.Devices;
            }
        }

        private void StartEdit() => IsEditing = true;

        private async Task CancelEdit()
        {
            IsEditing = false;
            await LoadUser();
            EditProfile = user is not null ? ProfileData.FromRecord(user) : null;
        }

        private async Task DisableTotp(TOTPDeviceLimited device)
        {
            var res = await userClient.DisableOwnTotpAsync(
                new DisableOwnTotpRequest { TotpID = device.TotpID },
                userHelper.GetGrpcCallOptions());

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("MFA device removed.");
                await GetUserTOTP();
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : res.Error.Reason.ToString();

                ToastService.Error(message);
            }
        }

        private async Task CancelSubscription((GenericSubscriptionFullRecord Subscription, string Reason) args)
        {
            var subscriptionId = args.Subscription.SubscriptionRecord?.InternalSubscriptionID;
            if (string.IsNullOrEmpty(subscriptionId))
            {
                return;
            }

            var reason = string.IsNullOrWhiteSpace(args.Reason) ? "User requested cancellation" : args.Reason;

            var res = await paymentClient.CancelSubscription(new CancelOwnSubscriptionRequest
            {
                InternalSubscriptionID = subscriptionId,
                Reason = reason,
            });

            if (res is not null && string.IsNullOrEmpty(res.Error))
            {
                ToastService.Success("Subscription canceled.");
                await GetUserSubs();
            }
            else
            {
                ToastService.Error(!string.IsNullOrEmpty(res?.Error) ? res.Error : "Failed to cancel subscription.");
            }
        }

        private async Task SaveUser()
        {
            if (EditProfile is null)
            {
                return;
            }

            var res = await userClient.ModifyOtherUserAsync(
                new ModifyOtherUserRequest
                {
                    UserID = user.Public.UserID,
                    UserName = EditProfile.UserName,
                    DisplayName = EditProfile.DisplayName,
                    Bio = EditProfile.Bio,
                    Email = EditProfile.Email,
                    FirstName = EditProfile.FirstName,
                    LastName = EditProfile.LastName,
                    PostalCode = EditProfile.PostalCode,
                },
                userHelper.GetGrpcCallOptions());

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("User updated successfully.");
                IsEditing = false;
                await LoadUser();
                EditProfile = user is not null ? ProfileData.FromRecord(user) : null;
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : res.Error.Reason.ToString();

                ToastService.Error(message);
            }
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

        private void OpenAddTotp()
        {
            newTotpDeviceName = string.Empty;
            newTotpCode = string.Empty;
            addTotpStep = 1;
            isAddTotpOpen = true;
        }

        private void CloseAddTotp() => isAddTotpOpen = false;

        private void NextAddTotpStep() => addTotpStep = 2;

        private void PreviousAddTotpStep() => addTotpStep = 1;

        private void ToggleResetPassword()
        {
            isResetPasswordOpen = !isResetPasswordOpen;

            if (isResetPasswordOpen)
            {
                oldPassword = string.Empty;
                newPassword = string.Empty;
                confirmNewPassword = string.Empty;
                passwordErrors = [];
            }
        }

        private void ClearPasswordValidation() => passwordErrors = [];

        private async Task SavePassword()
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                passwordErrors = ["Current and new password are required."];
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                passwordErrors = ["Passwords do not match."];
                return;
            }

            var req = new ChangeOwnPasswordRequest
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
            };

            var validation = Validator.Validate(req, failFast: false);
            passwordErrors = validation.Violations.ForField("NewPassword").Errors;

            if (!validation.IsSuccess)
            {
                return;
            }

            isSavingPassword = true;

            var res = await userClient.ChangeOwnPasswordAsync(req, userHelper.GetGrpcCallOptions());

            isSavingPassword = false;

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("Password updated successfully.");
                isResetPasswordOpen = false;
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : res.Error.Reason.ToString();

                passwordErrors = [message];
                ToastService.Error(message);
            }
        }
    }

    public class ProfileData
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public byte[] ProfileImagePNG { get; set; } = [];
        public List<string> Roles { get; set; } = [];
        public DateTime? CreatedOnUTC { get; set; }

        public static ProfileData FromRecord(UserNormalRecord record) => new()
        {
            UserId = record.Public?.UserID ?? string.Empty,
            DisplayName = record.Public?.Data?.DisplayName ?? string.Empty,
            UserName = record.Public?.Data?.UserName ?? string.Empty,
            Bio = record.Public?.Data?.Bio ?? string.Empty,
            Email = record.Private?.Data?.Email ?? string.Empty,
            FirstName = record.Private?.Data?.FirstName ?? string.Empty,
            LastName = record.Private?.Data?.LastName ?? string.Empty,
            PostalCode = record.Private?.Data?.PostalCode ?? string.Empty,
            ProfileImagePNG = record.Public?.Data?.ProfileImagePNG?.ToByteArray() ?? [],
            Roles = record.Private?.Roles?.ToList() ?? [],
            CreatedOnUTC = record.Public?.CreatedOnUTC?.ToDateTime(),
        };
    }
}
