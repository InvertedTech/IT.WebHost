using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Careers;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Careers
{
    public partial class ListCareers
    {
        [Inject]
        public ONUserHelper UserHelper { get; set; } = null!;
        [Inject]
        public CareersInterface.CareersInterfaceClient CareerClient { get; set; } = null!;

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

        [SupplyParameterFromQuery(Name = "deleted")]
        public bool? IncludeDeletedFilter { get; set; }

        private bool isLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;
        public List<CareerListRecord> CareerList { get; private set; } = new List<CareerListRecord>();

        private bool _includeDeletedFilter;

        private string ExtraQuery => IncludeDeletedFilter == true ? "deleted=true" : "";

        override protected async Task OnParametersSetAsync()
        {
            _includeDeletedFilter = IncludeDeletedFilter ?? false;
            await GetCareers();
        }

        public async Task GetCareers()
        {
            isLoading = true;
            var req = new AdminListCareersRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset,
                IncludeDeleted = IncludeDeletedFilter ?? false,
            };
            var res = await CareerClient.AdminListCareersAsync(req, UserHelper.GetGrpcCallOptions());
            CareerList = res.Careers.ToList();
            totalItems = res.PageTotalItems;
            isLoading = false;
            StateHasChanged();
        }

        private void ToggleIncludeDeleted(bool value)
        {
            var parts = new List<string> { $"size={pageSize}" };
            if (value)
                parts.Add("deleted=true");

            Nav.NavigateTo($"/careers?{string.Join("&", parts)}");
        }
    }
}
