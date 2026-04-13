using IT.WebHost.Core.Models;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class ModifyUser
    {
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
        [Inject] private AdminPaymentInterface.AdminPaymentInterfaceClient AdminPaymentClient { get; set; } = null!;
        [Parameter] public string UserID { get; set; } = null!;

        private UserNormalRecord? User { get; set; } = null;
        private IEnumerable<GenericSubscriptionFullRecord> SubscriptionRecords { get; set; } = new List<GenericSubscriptionFullRecord>();
        private IEnumerable<TOTPDeviceLimited> TOTPDevices { get; set; } = new List<TOTPDeviceLimited>();
        private bool _isLoading = true;
        private bool _isEditing = false;
        private ProfileData? _editProfile;

        protected override async Task OnParametersSetAsync()
        {
            _isLoading = true;
            _isEditing = false;
            User = null;
            await LoadUser();
            _editProfile = User is not null ? ProfileData.FromRecord(User) : null;
            _isLoading = false;
        }

        private Task LoadUser()
            => Task.WhenAll(LoadUserData(), LoadSubs(), LoadTOTP());

        private async Task LoadUserData()
        {
            var req = new GetOtherUserRequest()
            {
                UserID = UserID,
            };

            var res = await UserClient.GetOtherUserAsync(req, UserHelper.GetGrpcCallOptions());

            if (res != null)
            {
                User = res.Record;
            }
        }

        private async Task LoadSubs()
        {
            var req = new GetOtherSubscriptionRecordsRequest()
            {
                UserID = UserID
            };

            var res = await AdminPaymentClient.GetOtherSubscriptionRecordsAsync(req, UserHelper.GetGrpcCallOptions());

            if (res != null)
            {
                SubscriptionRecords = res.Generic.ToList();
            }
        }

        private async Task LoadTOTP()
        {
            var req = new GetOtherTotpListRequest()
            {
                UserID = UserID
            };

            var res = await UserClient.GetOtherTotpListAsync(req, UserHelper.GetGrpcCallOptions());

            if ( res != null )
            {
                TOTPDevices = res.Devices;
            }
        }

        private void StartEdit() => _isEditing = true;

        private async Task CancelEdit()
        {
            _isEditing = false;
            await LoadUser();
            _editProfile = User is not null ? ProfileData.FromRecord(User) : null;
        }

        private Task SaveUser()
        {
            _isEditing = false;
            // TODO: wire up gRPC save call
            return Task.CompletedTask;
        }

        private static string GetInitials(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) return "?";
            var parts = displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}"
                : displayName[..Math.Min(2, displayName.Length)].ToUpper();
        }
    }
}
