using IT.WebServices.Authentication;
using IT.WebServices.Clients.Payment;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages
{
    public partial class Profile
    {
        [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
        // [Inject] private PaymentInterface.PaymentInterfaceClient PaymentClient { get; set; } = null!;
        [Inject] private PaymentClient PaymentClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;

        private ONUser? User => UserHelper.MyUser;
        private UserNormalRecord? UserRecord { get; set; }
        private IEnumerable<GenericSubscriptionFullRecord> SubscriptionRecords { get; set; } = new List<GenericSubscriptionFullRecord>();

        protected override async Task OnInitializedAsync()
        {
            await Task.WhenAll(
                    LoadUser(),
                    LoadSubs()
                    );
        }

        private async Task LoadUser()
        {
            try
            {
                var res = await UserClient.GetOwnUserAsync(new GetOwnUserRequest(), UserHelper.GetGrpcCallOptions());
                if (res is null)
                {
                    Console.WriteLine("No User Response");
                    return;
                }

                UserRecord = res.Record;
                return;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return;
            }
        }

        private async Task LoadSubs(CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await PaymentClient.GetOwnSubscriptions(new GetOwnSubscriptionRecordsRequest
                {
                }, cancellationToken);

                if (res is null)
                {
                    Console.WriteLine("No subscription Response");
                    return;
                }

                SubscriptionRecords = res.Generic.ToList();
                return;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return;
            }
        }

        private async Task HandleModifyOwnUser(ModifyOwnUserRequest req)
        {
            await ModifyOwnUser(req);
            await LoadUser();
        }

        private async Task HandleCancel(GenericSubscriptionFullRecord record)
        {
            await HandleCancelSubscription(new CancelOwnSubscriptionRequest
            {
                InternalSubscriptionID = record.SubscriptionRecord.InternalSubscriptionID,
            });
        }

        private async Task HandleCancelSubscription(CancelOwnSubscriptionRequest req)
        {
            await PaymentClient.CancelSubscription(req);
            await LoadSubs();
            StateHasChanged();
        }

        private async Task<ModifyOwnUserResponse> ModifyOwnUser(ModifyOwnUserRequest req, CancellationToken cancellationToken = default)
        {
            try
            {
                var res = await UserClient.ModifyOwnUserAsync(req, UserHelper.GetGrpcCallOptions(cancellationToken));
                StateHasChanged();
                return res;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return new();
            }
        }
    }
}
