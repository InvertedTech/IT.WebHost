using Google.Protobuf.WellKnownTypes;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Users
{
    public partial class Users
    {
        [Inject] private UserInterface.UserInterfaceClient UsersClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private NavigationManager Nav { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        public List<UserSearchRecord> UserList { get; private set; } = new List<UserSearchRecord>();
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

        [SupplyParameterFromQuery(Name = "q")]
        public string? SearchFilter { get; set; }

        [SupplyParameterFromQuery(Name = "roles")]
        public string? RolesFilter { get; set; }

        [SupplyParameterFromQuery(Name = "from")]
        public string? CreatedAfterFilter { get; set; }

        [SupplyParameterFromQuery(Name = "to")]
        public string? CreatedBeforeFilter { get; set; }

        [SupplyParameterFromQuery(Name = "deleted")]
        public bool? IncludeDeletedFilter { get; set; }

        private bool IsLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;

        private string _searchFilter = string.Empty;
        private List<string> _rolesFilter = new();
        private DateOnly? _createdAfterFilter;
        private DateOnly? _createdBeforeFilter;
        private bool _includeDeletedFilter;

        private string ExtraQuery
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(SearchFilter))
                    parts.Add($"q={Uri.EscapeDataString(SearchFilter)}");
                if (!string.IsNullOrEmpty(RolesFilter))
                    parts.Add($"roles={Uri.EscapeDataString(RolesFilter)}");
                if (!string.IsNullOrEmpty(CreatedAfterFilter))
                    parts.Add($"from={Uri.EscapeDataString(CreatedAfterFilter)}");
                if (!string.IsNullOrEmpty(CreatedBeforeFilter))
                    parts.Add($"to={Uri.EscapeDataString(CreatedBeforeFilter)}");
                if (IncludeDeletedFilter == true)
                    parts.Add("deleted=true");
                return string.Join("&", parts);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            // TODO: Guard this page - calls SearchUsersAdmin which requires ROLE_IS_MEMBER_MANAGER_OR_HIGHER ("member_manager,admin,owner"); add <AuthorizeView Roles="member_manager,admin,owner"> around content in .razor
            _searchFilter = SearchFilter ?? string.Empty;
            _rolesFilter = string.IsNullOrEmpty(RolesFilter) ? new() : RolesFilter.Split(',').ToList();
            _createdAfterFilter = DateOnly.TryParse(CreatedAfterFilter, out var after) ? after : null;
            _createdBeforeFilter = DateOnly.TryParse(CreatedBeforeFilter, out var before) ? before : null;
            _includeDeletedFilter = IncludeDeletedFilter ?? false;

            await GetUsers();
        }

        public async Task GetUsers()
        {
            IsLoading = true;
            var req = new SearchUsersAdminRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset,
                IncludeDeleted = IncludeDeletedFilter ?? false,
            };

            if (!string.IsNullOrEmpty(SearchFilter))
                req.SearchString = SearchFilter;

            if (!string.IsNullOrEmpty(RolesFilter))
                req.Roles.AddRange(RolesFilter.Split(','));

            if (DateOnly.TryParse(CreatedAfterFilter, out var createdAfter))
                req.CreatedAfter = Timestamp.FromDateTime(createdAfter.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime());

            if (DateOnly.TryParse(CreatedBeforeFilter, out var createdBefore))
                req.CreatedBefore = Timestamp.FromDateTime(createdBefore.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime());

            var res = await UsersClient.SearchUsersAdminAsync(req, UserHelper.GetGrpcCallOptions());

            UserList = res.Records.ToList();
            totalItems = res.PageTotalItems;
            IsLoading = false;
            StateHasChanged();
        }

        private void ApplyFilters(string search, List<string> roles, DateOnly? createdAfter, DateOnly? createdBefore, bool includeDeleted)
        {
            var parts = new List<string> { $"size={pageSize}" };
            if (!string.IsNullOrEmpty(search))
                parts.Add($"q={Uri.EscapeDataString(search)}");
            if (roles.Count > 0)
                parts.Add($"roles={Uri.EscapeDataString(string.Join(',', roles))}");
            if (createdAfter.HasValue)
                parts.Add($"from={Uri.EscapeDataString(createdAfter.Value.ToString("yyyy-MM-dd"))}");
            if (createdBefore.HasValue)
                parts.Add($"to={Uri.EscapeDataString(createdBefore.Value.ToString("yyyy-MM-dd"))}");
            if (includeDeleted)
                parts.Add("deleted=true");

            Nav.NavigateTo($"/users?{string.Join("&", parts)}");
        }

        private void ClearFilters() => Nav.NavigateTo("/users");
    }
}
