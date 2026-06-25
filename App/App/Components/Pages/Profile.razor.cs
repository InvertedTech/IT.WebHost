using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace WebApp.Components.Pages
{
    public partial class Profile
    {
        [Inject] public ONUserHelper userHelper { get; set; } = null!;
        [Inject] public UserInterface.UserInterfaceClient userClient { get; set; } = null!;

        private UserNormalRecord? user { get; set; }
        private IEnumerable<GenericSubscriptionFullRecord> SubscriptionRecords { get; set; } = [];
        private IEnumerable<TOTPDeviceLimited> TOTPDevices { get; set; } = [];
        private ProfileData? EditProfile { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsEditing { get; set; }
        private bool isResetPasswordOpen { get; set; } = false;

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
            await Task.WhenAll(GetUser(), GetUserSubs(), GetUserTOTP());
        }

        private async Task GetUser()
        {
            var res = await userClient.GetOtherUserAsync(
                new GetOtherUserRequest { UserID = userHelper.MyUserId.ToString() },
                userHelper.GetGrpcCallOptions());

            user = res?.Record;
        }

        // TODO: Figure Out Why This Isn't Working
        private async Task GetUserSubs()
        {
            //var res = await PaymentClient.GetOtherSubscriptionRecordsAsync(
            //    new GetOtherSubscriptionRecordsRequest { UserID = Id },
            //    ONUserHelper.GetGrpcCallOptions());

            //if (res is not null)
            //{
            //    SubscriptionRecords = res.Generic.ToList();
            //}
        }

        private async Task GetUserTOTP()
        {
            var res = await userClient.GetOtherTotpListAsync(
                new GetOtherTotpListRequest { UserID = userHelper.MyUserId.ToString() },
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

        //private async Task DisableTotp(TOTPDeviceLimited device)
        //{
        //    var res = await UsersClient.DisableOtherTotpAsync(
        //        new DisableOtherTotpRequest
        //        {
        //            UserID = Id,
        //            TotpID = device.TotpID,
        //        },
        //        ONUserHelper.GetGrpcCallOptions());

        //    if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
        //    {
        //        ToastService.Success("MFA device removed.");
        //        await GetUserTOTP();
        //    }
        //    else
        //    {
        //        var message = !string.IsNullOrEmpty(res?.Error?.Message)
        //            ? res.Error.Message
        //            : res?.Error?.Reason.ToString() ?? "Failed to remove device.";

        //        ToastService.Error(message);
        //    }
        //}

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
                //ToastService.Success("User updated successfully.");
                IsEditing = false;
                await LoadUser();
                EditProfile = user is not null ? ProfileData.FromRecord(user) : null;
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : res.Error.Reason.ToString();

                //ToastService.Error(message);
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
