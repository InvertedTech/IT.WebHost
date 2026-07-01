using Admin.Models;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Users
{
    public partial class ViewUser
    {
        [Parameter]
        public string Id { get; set; } = null!;

        [Inject] private UserInterface.UserInterfaceClient UsersClient { get; set; } = null!;
        [Inject] private AdminPaymentInterface.AdminPaymentInterfaceClient PaymentClient { get; set; } = null!;
        [Inject] private ONUserHelper ONUserHelper { get; set; } = null!;
        [Inject] private IToastService ToastService { get; set; } = null!;

        private UserNormalRecord? User { get; set; }
        private IEnumerable<GenericSubscriptionFullRecord> SubscriptionRecords { get; set; } = [];
        private IEnumerable<TOTPDeviceLimited> TOTPDevices { get; set; } = [];
        private ProfileData? EditProfile { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsEditing { get; set; }
        private bool isResetPasswordOpen { get; set; } = false;

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;
            IsEditing = false;
            User = null;
            // TODO: This page requires member_manager,admin,owner roles (ROLE_IS_MEMBER_MANAGER_OR_HIGHER) for all the UserClient calls; wrap corresponding UI sections in AuthorizeView
            await LoadUser();
            EditProfile = User is not null ? ProfileData.FromRecord(User) : null;
            IsLoading = false;
        }

        private async Task LoadUser()
        {
            await Task.WhenAll(GetUser(), GetUserSubs(), GetUserTOTP());
        }

        private async Task GetUser()
        {
            var res = await UsersClient.GetOtherUserAsync(
                new GetOtherUserRequest { UserID = Id },
                ONUserHelper.GetGrpcCallOptions());

            User = res?.Record;
        }

        // TODO: Figure Out Why This Isn't Working
        private async Task GetUserSubs()
        {
            var res = await PaymentClient.GetOtherSubscriptionRecordsAsync(
                new GetOtherSubscriptionRecordsRequest { UserID = Id },
                ONUserHelper.GetGrpcCallOptions());

            if (res is not null)
            {
                SubscriptionRecords = res.Generic.ToList();
            }
        }

        private async Task GetUserTOTP()
        {
            var res = await UsersClient.GetOtherTotpListAsync(
                new GetOtherTotpListRequest { UserID = Id },
                ONUserHelper.GetGrpcCallOptions());

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
            EditProfile = User is not null ? ProfileData.FromRecord(User) : null;
        }

        private async Task SaveUser()
        {
            if (EditProfile is null)
            {
                return;
            }

            var res = await UsersClient.ModifyOtherUserAsync(
                new ModifyOtherUserRequest
                {
                    UserID = Id,
                    UserName = EditProfile.UserName,
                    DisplayName = EditProfile.DisplayName,
                    Bio = EditProfile.Bio,
                    Email = EditProfile.Email,
                    FirstName = EditProfile.FirstName,
                    LastName = EditProfile.LastName,
                    PostalCode = EditProfile.PostalCode,
                },
                ONUserHelper.GetGrpcCallOptions());

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("User updated successfully.");
                IsEditing = false;
                await LoadUser();
                EditProfile = User is not null ? ProfileData.FromRecord(User) : null;
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : res.Error.Reason.ToString();

                ToastService.Error(message);
            }
        }

        private async Task DisableTotp(TOTPDeviceLimited device)
        {
            var res = await UsersClient.DisableOtherTotpAsync(
                new DisableOtherTotpRequest
                {
                    UserID = Id,
                    TotpID = device.TotpID,
                },
                ONUserHelper.GetGrpcCallOptions());

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("MFA device removed.");
                await GetUserTOTP();
            }
            else
            {
                var message = !string.IsNullOrEmpty(res?.Error?.Message)
                    ? res.Error.Message
                    : res?.Error?.Reason.ToString() ?? "Failed to remove device.";

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

        private void ToggleResetPassword()
        {
            isResetPasswordOpen = !isResetPasswordOpen;
        }
    }
}