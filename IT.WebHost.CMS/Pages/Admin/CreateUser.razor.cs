using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class CreateUser
    {
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private UserInterface.UserInterfaceClient UserClient { get; set; } = null!;

    }
}
