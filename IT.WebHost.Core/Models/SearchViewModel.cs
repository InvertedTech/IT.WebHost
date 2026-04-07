using IT.WebServices.Fragments.Content;

namespace IT.WebHost.Core.Models
{
    public class SearchViewModel
    {
        public string? Query { get; set; } = string.Empty;
        public uint? PageSize { get; set; } = 10;
        public uint? PageOffset { get; set; } = 0;
        public ContentType? ContentType { get; set; } = WebServices.Fragments.Content.ContentType.ContentNone;
        public string? CategoryId { get; set; } = string.Empty;
        public string? ChannelId { get; set; } = string.Empty;
        public string? Tags { get; set; } = string.Empty;
        public bool? OnlyLive { get; set; } = false;
        public IEnumerable<ContentListRecord> Content { get; set; } = [];
        public IReadOnlyList<string> TagsParsed => string.IsNullOrWhiteSpace(Tags)
        ? []
        : Tags.Trim('[', ']').Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(Uri.UnescapeDataString)
              .ToList();
    }
}