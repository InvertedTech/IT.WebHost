using IT.WebServices.Authentication;
using IT.WebServices.Fragments.AuditLog;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages
{
    public partial class AuditLog
    {
        [Inject]
        public ONUserHelper UserHelper { get; set; } = null!;
        [Inject]
        public AuditLogInterface.AuditLogInterfaceClient AuditLogClient { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
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
        public List<AuditLogEntry> Entries { get; private set; } = new List<AuditLogEntry>();

        private AuditLogEntry? selectedEntry { get; set; }
        private bool isDetailsOpen { get; set; }

        override protected async Task OnParametersSetAsync()
        {
            await GetEntries();
        }

        public async Task GetEntries()
        {
            isLoading = true;
            var req = new SearchEntriesRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset
            };
            var res = await AuditLogClient.SearchEntriesAsync(req, UserHelper.GetGrpcCallOptions());
            Entries = res.Entries.ToList();
            totalItems = res.PageTotalItems;
            isLoading = false;
            StateHasChanged();
        }

        private void ViewDetails(AuditLogEntry entry)
        {
            selectedEntry = entry;
            isDetailsOpen = true;
        }

        private void CloseDetails()
        {
            isDetailsOpen = false;
        }
    }
}
