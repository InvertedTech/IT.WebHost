using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Dashboard;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Components.Admin.Dashboard
{
    public partial class UserStatsWidget : ComponentBase
    {
        [Inject] private DashboardInterface.DashboardInterfaceClient DashboardClient { get; set; } = default!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;

        private GetKpisResponse? _data;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _data = await DashboardClient.GetKpisAsync(new GetKpisRequest(), UserHelper.GetGrpcCallOptions());
            }
            catch { }
        }

        private static string FormatRatio(double ratio) =>
            (ratio * 100).ToString("F1") + "%";

        private static string FormatChange(double pct) =>
            pct >= 0 ? $"+{pct:F2}% from last month" : $"{pct:F2}% from last month";

        private static string ChangeColor(double pct, bool inverse = false) =>
            (inverse ? pct <= 0 : pct >= 0) ? "text-emerald-500" : "text-red-500";
    }
}
