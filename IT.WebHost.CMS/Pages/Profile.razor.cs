using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages
{
    public partial class Profile
    {
        [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;
        [Inject] private PaymentInterface.PaymentInterfaceClient PaymentClient { get; set; } = null!;
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

        private async Task LoadSubs()
        {
            try
            {
                var res = await PaymentClient.GetOwnSubscriptionRecordsAsync(new GetOwnSubscriptionRecordsRequest
                {
                }, UserHelper.GetGrpcCallOptions());

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
    }
}
