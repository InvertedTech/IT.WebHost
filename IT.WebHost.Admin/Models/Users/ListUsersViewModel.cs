using IT.WebServices.Fragments.Authentication;

namespace IT.WebHost.Admin.Models.Users
{
    public class ListUsersViewModel
    {
        public IEnumerable<UserRecord> Users { get; set; } = new List<UserRecord>();
        public string? PageSize { get; set; } = "10";
        public string? PageOffset { get; set; } = "0";
        public string? PageTotalItems { get; set; } = "0";
        private uint _pageSize => uint.TryParse(PageSize, out var v) ? v : 10;
        private uint _pageOffset => uint.TryParse(PageOffset, out var v) ? v : 0;
        private uint _pageTotalItems => uint.TryParse(PageTotalItems, out var v) ? v : 0;
    }
}
