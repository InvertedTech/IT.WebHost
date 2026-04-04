using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;

namespace IT.WebHost.Core.Clients
{
    public class AuthClient
    {
        private readonly UserInterface.UserInterfaceClient client;
        private readonly ONUserHelper userHelper;

        public AuthClient(UserInterface.UserInterfaceClient client, ONUserHelper userHelper)
        {
            this.client = client;
            this.userHelper = userHelper;
        }

        public async Task<AuthenticateUserResponse> LoginAsync(AuthenticateUserRequest request)
        {
            return await client.AuthenticateUserAsync(request);
        }

        public async Task<CreateUserResponse> SignUpAsync(CreateUserRequest request)
        {
            return await client.CreateUserAsync(request);
        }

        public async Task<GetOwnUserResponse> GetOwnUserAsync()
        {
            return await client.GetOwnUserAsync(new(), userHelper.GetGrpcCallOptions());
        }

        public async Task<ModifyOwnUserResponse> ModifyOwnUserAsync(ModifyOwnUserRequest request)
        {
            return await client.ModifyOwnUserAsync(request, userHelper.GetGrpcCallOptions());
        }

        public async Task<ChangeOwnPasswordResponse> ChangeOwnPasswordAsync(ChangeOwnPasswordRequest request)
        {
            return await client.ChangeOwnPasswordAsync(request, userHelper.GetGrpcCallOptions());
        }
    }
}
