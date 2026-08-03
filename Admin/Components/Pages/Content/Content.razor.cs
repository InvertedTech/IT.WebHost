using IT.WebServices.Clients.CMS;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Content;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Content
{
    public partial class Content
    {
        [Inject] private ContentClient ContentClient { get; set; } = null!;
        [Inject] private ChannelHelper ChannelHelper { get; set; } = null!;
        [Inject] private CategoryHelper CategoryHelper { get; set; } = null!;

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

        [SupplyParameterFromQuery(Name = "type")]
        public string? ContentTypeFilter { get; set; }

        [SupplyParameterFromQuery(Name = "channel")]
        public string? ChannelFilter { get; set; }

        [SupplyParameterFromQuery(Name = "category")]
        public string? CategoryFilter { get; set; }

        [SupplyParameterFromQuery(Name = "live")]
        public bool? OnlyLiveFilter { get; set; }

        [SupplyParameterFromQuery(Name = "deleted")]
        public bool? IncludeDeletedFilter { get; set; }

        private bool isLoading { get; set; } = true;
        private uint totalItems { get; set; } = 0;

        private ChannelRecord[] Channels { get; set; } = [];
        private CategoryRecord[] Categories { get; set; } = [];

        private string? _typeFilter;
        private string? _channelFilter;
        private string? _categoryFilter;
        private bool _onlyLiveFilter;
        private bool _includeDeletedFilter;

        private string ExtraQuery
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrEmpty(ContentTypeFilter))
                    parts.Add($"type={Uri.EscapeDataString(ContentTypeFilter)}");
                if (!string.IsNullOrEmpty(ChannelFilter))
                    parts.Add($"channel={Uri.EscapeDataString(ChannelFilter)}");
                if (!string.IsNullOrEmpty(CategoryFilter))
                    parts.Add($"category={Uri.EscapeDataString(CategoryFilter)}");
                if (OnlyLiveFilter == true)
                    parts.Add("live=true");
                if (IncludeDeletedFilter == true)
                    parts.Add("deleted=true");
                return string.Join("&", parts);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            Channels = ChannelHelper.GetAll();
            Categories = CategoryHelper.GetAll();

            _typeFilter = ContentTypeFilter;
            _channelFilter = ChannelFilter;
            _categoryFilter = CategoryFilter;
            _onlyLiveFilter = OnlyLiveFilter ?? false;
            _includeDeletedFilter = IncludeDeletedFilter ?? false;

            await GetContent();
        }

        public async Task GetContent()
        {
            isLoading = true;

            var req = new GetAllContentAdminRequest
            {
                PageSize = (uint)pageSize,
                PageOffset = (uint)pageOffset,
                OnlyLive = OnlyLiveFilter ?? false,
                Deleted = IncludeDeletedFilter ?? false,
            };

            if (!string.IsNullOrEmpty(ContentTypeFilter) && Enum.TryParse<ContentType>(ContentTypeFilter, out var contentType))
                req.ContentType = contentType;

            if (!string.IsNullOrEmpty(ChannelFilter))
                req.ChannelId = ChannelFilter;

            if (!string.IsNullOrEmpty(CategoryFilter))
                req.CategoryId = CategoryFilter;

            var res = await ContentClient.GetAllContentAdmin(req);
            ContentList = res.Records.ToList();
            totalItems = res.PageTotalItems;
            isLoading = false;
            StateHasChanged();
        }

        private void ApplyFilters(string? contentType, string? channel, string? category, bool onlyLive, bool includeDeleted)
        {
            var parts = new List<string> { $"size={pageSize}" };
            if (!string.IsNullOrEmpty(contentType))
                parts.Add($"type={Uri.EscapeDataString(contentType)}");
            if (!string.IsNullOrEmpty(channel))
                parts.Add($"channel={Uri.EscapeDataString(channel)}");
            if (!string.IsNullOrEmpty(category))
                parts.Add($"category={Uri.EscapeDataString(category)}");
            if (onlyLive)
                parts.Add("live=true");
            if (includeDeleted)
                parts.Add("deleted=true");

            Nav.NavigateTo($"/content?{string.Join("&", parts)}");
        }

        private void ClearFilters() => Nav.NavigateTo("/content");
    }
}
