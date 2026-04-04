using Google.Protobuf;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using System.Reflection;
using System.Text;
using static Google.Rpc.Context.AttributeContext.Types;

namespace IT.WebHost.Core.Clients
{
    public class ContentClient
    {
        private readonly ContentInterface.ContentInterfaceClient client;
        private readonly ONUserHelper userHelper;

        public ContentClient(ContentInterface.ContentInterfaceClient client, ONUserHelper userHelper)
        {
            this.client = client;
            this.userHelper = userHelper;
        }

        public async Task<GetAllContentResponse> GetAllContentAsync(GetAllContentRequest request)
        {
            return await client.GetAllContentAsync(request, userHelper.GetGrpcCallOptions());
        }

        public async Task<GetContentResponse> GetContentByIdAsync(string contentId)
        {
            return await client.GetContentAsync(new() { ContentID = contentId }, userHelper.GetGrpcCallOptions());
        }
    }
}
