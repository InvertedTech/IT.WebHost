using Google.Protobuf.WellKnownTypes;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Dashboard;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages
{
    public partial class Home
    {
        [Inject] private ONUserHelper ONUserHelper { get; set; } = null!;
        [Inject] private DashboardInterface.DashboardInterfaceClient DashboardClient { get; set; } = null!;

        private bool _isLoading = true;

        private UserKpis _userKpis { get; set; } = new()
        {
            TotalUsers = new(),
            NewUsers = new(),
            DisabledUsers = new(),
            ChurnRate = new()
        };

        private SubscriptionKpis _subscriptionKpis { get; set; } = new()
        {
            ActiveSubscriptions = new(),
            NewSubscriptions = new(),
            CanceledSubscriptions = new(),
            NewSubscriptionRevenue = new(),
            TotalSubscriptionRevenue = new()
        };

        private ContentKpis _contentKpis { get; set; } = new()
        {
            UniqueViewers = new(),
            CompletionRate = new(),
            MedianCompletionPercent = new(),
            AvgConsumptionSeconds = new()
        };

        private DateTime? _asOfUtc;

        private List<SubscriptionActivityPoint> _subscriptionActivity = new();
        private List<RevenueSeriesPoint> _revenueSeries = new();
        private List<NamedValuePoint> _topPlansChart = new();
        private List<NamedValuePoint> _topContentChart = new();
        private List<UserComparisonPoint> _userComparison = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadKpis();
        }

        private async Task LoadKpis()
        {
            _isLoading = true;

            try
            {
                var res = await DashboardClient.GetKpisAsync(new GetKpisRequest(), ONUserHelper.GetGrpcCallOptions());
                if (res is not null)
                {
                    _userKpis = res.Users ?? _userKpis;
                    _subscriptionKpis = res.Subscriptions ?? _subscriptionKpis;
                    _contentKpis = res.Content ?? _contentKpis;
                    _asOfUtc = res.AsOfUTC?.ToDateTime();
                    BuildChartData();
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void BuildChartData()
        {
            _subscriptionActivity = BuildSubscriptionActivity();
            _revenueSeries = _subscriptionKpis.TotalRevenueSeries
                .Select(point => new RevenueSeriesPoint(
                    FormatBucketLabel(point.BucketStart),
                    point.ValueCents / 100m))
                .ToList();

            _topPlansChart = _subscriptionKpis.TopPlansByRevenue
                .Select(plan => new NamedValuePoint(
                    plan.PlanName,
                    plan.RevenueCents / 100m))
                .ToList();

            _topContentChart = _contentKpis.TopLikedContent
                .Select(item => new NamedValuePoint(
                    TruncateLabel(item.ContentTitle, 28),
                    item.LikeCount))
                .ToList();

            _userComparison =
            [
                new UserComparisonPoint("Total Users", _userKpis.TotalUsers.CurrentCount, _userKpis.TotalUsers.PreviousCount),
                new UserComparisonPoint("New Users", _userKpis.NewUsers.CurrentCount, _userKpis.NewUsers.PreviousCount),
                new UserComparisonPoint("Disabled Users", _userKpis.DisabledUsers.CurrentCount, _userKpis.DisabledUsers.PreviousCount),
            ];
        }

        private List<SubscriptionActivityPoint> BuildSubscriptionActivity()
        {
            var newSeries = _subscriptionKpis.NewSubscriptionsSeries;
            var canceledSeries = _subscriptionKpis.CanceledSubscriptionsSeries;
            var count = Math.Max(newSeries.Count, canceledSeries.Count);
            var points = new List<SubscriptionActivityPoint>(count);

            for (var i = 0; i < count; i++)
            {
                var bucket = i < newSeries.Count
                    ? newSeries[i].BucketStart
                    : canceledSeries[i].BucketStart;

                points.Add(new SubscriptionActivityPoint(
                    FormatBucketLabel(bucket),
                    i < newSeries.Count ? newSeries[i].Value : 0,
                    i < canceledSeries.Count ? canceledSeries[i].Value : 0));
            }

            return points;
        }

        private static string FormatBucketLabel(Timestamp? bucketStart)
        {
            if (bucketStart is null)
            {
                return string.Empty;
            }

            return bucketStart.ToDateTime().ToLocalTime().ToString("MMM d");
        }

        private static string TruncateLabel(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value[..(maxLength - 1)] + "…";
        }

        private static string FormatCount(long value) => value.ToString("N0");

        private static string FormatCurrencyFromCents(long cents) => (cents / 100m).ToString("C0");

        private static string FormatPercent(double ratio) => (ratio * 100).ToString("0.#") + "%";

        private static string FormatChange(double change)
        {
            var sign = change > 0 ? "+" : string.Empty;
            return $"{sign}{change:0.#}%";
        }

        private static string GetChangeClass(double change) => change switch
        {
            > 0 => "text-emerald-600 dark:text-emerald-400",
            < 0 => "text-rose-600 dark:text-rose-400",
            _ => "text-muted-foreground"
        };

        private sealed record SubscriptionActivityPoint(string Label, long NewSubscriptions, long CanceledSubscriptions);

        private sealed record RevenueSeriesPoint(string Label, decimal Revenue);

        private sealed record NamedValuePoint(string Name, decimal Value);

        private sealed record UserComparisonPoint(string Metric, long Current, long Previous);
    }
}