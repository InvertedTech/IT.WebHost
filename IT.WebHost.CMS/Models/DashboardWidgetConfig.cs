namespace IT.WebHost.CMS.Models
{
    public class DashboardWidgetConfig
    {
        public string WidgetId { get; set; } = "";
        public int Col { get; set; }
        public int Row { get; set; }
        public int ColSpan { get; set; } = 2;
        public int RowSpan { get; set; } = 1;
    }

}
