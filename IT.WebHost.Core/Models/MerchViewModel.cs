using IT.WebServices.Fragments.Merch;

namespace IT.WebHost.Core.Models
{
    public class MerchViewModel
    {
        public string? Query { get; set; }
        public string? PageSize { get; set; }
        public string? PageOffset { get; set; }
        public string? InternalStoreId { get; set; }
        public string? Tag { get; set; }
        public string? OnlyInStock { get; set; }
        public IEnumerable<GenericMerchRecord> Records { get; set; } = new List<GenericMerchRecord>();
        public uint PageTotalItems { get; set; } = 0;

        public uint PageSizeParsed => uint.TryParse(PageSize, out var v) ? v : 10;
        public uint PageOffsetParsed => uint.TryParse(PageOffset, out var v) ? v : 0;
        public bool OnlyInStockParsed => bool.TryParse(OnlyInStock, out var v) && v;
        public string BaseMerchUrl
        {
            get
            {
                var qs = $"q={Uri.EscapeDataString(Query ?? string.Empty)}";
                if (!string.IsNullOrEmpty(Tag))
                    qs += $"&tag={Uri.EscapeDataString(Tag)}";
                if (OnlyInStockParsed)
                    qs += "&onlyInStock=true";
                return $"/merch?{qs}";
            }
        }
    }
}
