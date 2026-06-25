using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Content
{
    public partial class Content
    {
        [Inject] private ContentClient ContentClient { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        public List<ContentListRecord> ContentList { get; private set; } = new List<ContentListRecord>();
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

        private bool isLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;

        protected override async Task OnParametersSetAsync()
        {
            // TODO: Guard page with AuthorizeView Roles using ROLE_CAN_CREATE_CONTENT ("con_publisher,con_writer,admin,owner") for GetAllContentAdmin
            await GetContent();
        }

        public async Task GetContent()
        {
            isLoading = true;

            var req = new GetAllContentAdminRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset
            };

            var res = await ContentClient.GetAllContentAdmin(req);
            ContentList = res.Records.ToList();
            totalItems = res.PageTotalItems;
            isLoading = false;
            StateHasChanged();
        }
    }
}
