using IT.WebServices.Fragments.Content;

namespace IT.WebHost.App.Models
{
    public class HomeViewModel
    {
        public List<ContentListRecord> ContentListRecords { get; set; } = new List<ContentListRecord>();
        public LayoutEnum Layout { get; set; } = LayoutEnum.List;
    }
}