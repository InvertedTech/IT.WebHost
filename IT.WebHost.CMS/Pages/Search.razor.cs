using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages
{
    public partial class Search
    {
        [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
        [Inject] private NavigationManager Nav { get; set; } = null!;

        public IEnumerable<ContentListRecord> ContentRecords { get; set; } = new List<ContentListRecord>();

        [SupplyParameterFromQuery(Name = "pageOffset")] public string? PageOffset { get; set; } = "0";
        [SupplyParameterFromQuery(Name = "pageSize")] public string? PageSize { get; set; } = "20";
        [SupplyParameterFromQuery(Name = "q")] public string? Query { get; set; }

        private uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
        private uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 20;
        private uint TotalItems { get; set; }

        private ContentType Type { get; set; } = ContentType.ContentNone;
        private string? CategoryId { get; set; }
        private string? ChannelId { get; set; }
        private string? Tags { get; set; }
        private bool OnlyLive { get; set; } = false;

        private IReadOnlyList<string> TagsParsed => string.IsNullOrWhiteSpace(Tags)
            ? []
            : Tags.Trim('[', ']').Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(Uri.UnescapeDataString)
                  .ToList();

        private string BaseUrl
        {
            get
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Query)) parts.Add($"q={Uri.EscapeDataString(Query)}");
                return parts.Count > 0 ? $"/search?{string.Join("&", parts)}" : "/search";
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadContent();
            StateHasChanged();
        }

        private async Task LoadContent()
        {
            var req = BuildRequest();
            var res = await ContentClient.SearchContentAsync(req);

            ContentRecords = res.Records;
            TotalItems = res.PageTotalItems;
        }

        private SearchContentRequest BuildRequest()
        {
            var req = new SearchContentRequest
            {
                PageSize = PageSizeParsed,
                PageOffset = PageOffsetParsed,
                OnlyLive = OnlyLive,
            };

            if (!string.IsNullOrEmpty(Query))
                req.Query = Query;

            if (Type != ContentType.ContentNone)
                req.ContentType = Type;

            if (CategoryId != null)
                req.CategoryId = CategoryId;

            if (ChannelId != null)
                req.ChannelId = ChannelId;

            if (Tags != null)
                req.Tag = Tags;

            return req;
        }
    }
}
