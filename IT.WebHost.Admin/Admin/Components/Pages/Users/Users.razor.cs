using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Users
{
    public partial class Users
    {
        [Inject] private UserInterface.UserInterfaceClient UsersClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        public List<UserNormalRecord> UserList { get; private set; } = new List<UserNormalRecord>();
        private int pageSize
        {
            get => int.Parse(PageSizeStr ?? "25");
        }

        [SupplyParameterFromQuery(Name = "offset")]
        public string? PageOffsetStr { get; set; }
        private int pageOffset
        {
            get => int.Parse(PageOffsetStr ?? "0");
        }

        private bool IsLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            // TODO: Guard this page - calls GetAllUsers which requires ROLE_IS_MEMBER_MANAGER_OR_HIGHER ("member_manager,admin,owner"); add <AuthorizeView Roles="member_manager,admin,owner"> around content in .razor
            await GetUsers();
        }

        public async Task GetUsers()
        {
            IsLoading = true;
            var req = new GetAllUsersRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset
            };

            var res = await UsersClient.GetAllUsersAsync(req, UserHelper.GetGrpcCallOptions());

            UserList = res.Records.ToList();
            totalItems = res.PageTotalItems;
            IsLoading = false;
            StateHasChanged();
        }
    }
}
