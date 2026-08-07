using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authorization.Events;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Events
{
    public partial class ListEvents
    {
        [Inject] public ONUserHelper UserHelper { get; set; } = null!;
        [Inject] public EventInterface.EventInterfaceClient EventClient { get; set; } = null!;

        [SupplyParameterFromQuery(Name = "size")]
        public string? PageSizeStr { get; set; }
        private int pageSize => int.Parse(PageSizeStr ?? "25");

        [SupplyParameterFromQuery(Name = "offset")]
        public string? PageOffsetStr { get; set; }
        private int pageOffset => int.Parse(PageOffsetStr ?? "0");

        private bool isLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;
        public List<GenericEventRecord> EventList { get; private set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await GetEvents();
        }

        public async Task GetEvents()
        {
            isLoading = true;

            var req = new GetEventsRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset,
            };

            var res = await EventClient.GetEventsAsync(req, UserHelper.GetGrpcCallOptions());
            EventList = res.Records.ToList();
            totalItems = res.PageTotalItems;

            isLoading = false;
            StateHasChanged();
        }
    }
}
