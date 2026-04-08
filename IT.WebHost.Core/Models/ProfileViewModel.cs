using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;

namespace IT.WebHost.Core.Models
{
    public class ProfileViewModel
    {
        public ProfileData? UserRecord { get; set; }
        public IEnumerable<SubscriptionData> SubscriptionRecords { get; set; } = [];
    }

    public class ProfileData
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public byte[] ProfileImagePNG { get; set; } = [];
        public List<string> Roles { get; set; } = [];
        public DateTime? CreatedOnUTC { get; set; }

        public static ProfileData FromRecord(UserNormalRecord record) => new()
        {
            UserId          = record.Public?.UserID             ?? string.Empty,
            DisplayName     = record.Public?.Data?.DisplayName  ?? string.Empty,
            UserName        = record.Public?.Data?.UserName     ?? string.Empty,
            Bio             = record.Public?.Data?.Bio          ?? string.Empty,
            Email           = record.Private?.Data?.Email       ?? string.Empty,
            FirstName       = record.Private?.Data?.FirstName   ?? string.Empty,
            LastName        = record.Private?.Data?.LastName    ?? string.Empty,
            PostalCode      = record.Private?.Data?.PostalCode  ?? string.Empty,
            ProfileImagePNG = record.Public?.Data?.ProfileImagePNG?.ToByteArray() ?? [],
            Roles           = record.Private?.Roles?.ToList()   ?? [],
            CreatedOnUTC    = record.Public?.CreatedOnUTC?.ToDateTime(),
        };
    }

    public class SubscriptionData
    {
        public string ProcessorName { get; set; } = string.Empty;
        public uint AmountCents { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? RenewsOn { get; set; }
        public DateTime? LastPaid { get; set; }
        public DateTime? PaidThru { get; set; }
        public List<PaymentData> Payments { get; set; } = [];

        public static SubscriptionData FromRecord(GenericSubscriptionFullRecord record) => new()
        {
            ProcessorName = record.SubscriptionRecord?.ProcessorName ?? string.Empty,
            AmountCents   = record.SubscriptionRecord?.AmountCents   ?? 0,
            Status        = FormatStatus(record.SubscriptionRecord?.Status),
            RenewsOn      = record.RenewsOnUTC?.ToDateTime(),
            LastPaid      = record.LastPaidUTC?.ToDateTime(),
            PaidThru      = record.PaidThruUTC?.ToDateTime(),
            Payments      = record.Payments.Select(PaymentData.FromRecord).ToList(),
        };

        private static string FormatStatus(SubscriptionStatus? status) => status switch
        {
            SubscriptionStatus.SubscriptionActive  => "Active",
            SubscriptionStatus.SubscriptionStopped => "Stopped",
            SubscriptionStatus.SubscriptionPending => "Pending",
            SubscriptionStatus.SubscriptionPaused  => "Paused",
            _                                      => "Unknown",
        };
    }

    public class PaymentData
    {
        public uint AmountCents { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ProcessorPaymentId { get; set; } = string.Empty;
        public DateTime? PaidOn { get; set; }
        public DateTime? PaidThru { get; set; }

        public static PaymentData FromRecord(GenericPaymentRecord record) => new()
        {
            AmountCents       = record.AmountCents,
            Status            = FormatStatus(record.Status),
            ProcessorPaymentId = record.ProcessorPaymentID,
            PaidOn            = record.PaidOnUTC?.ToDateTime(),
            PaidThru          = record.PaidThruUTC?.ToDateTime(),
        };

        private static string FormatStatus(PaymentStatus status) => status switch
        {
            PaymentStatus.PaymentComplete  => "Complete",
            PaymentStatus.PaymentPending   => "Pending",
            PaymentStatus.PaymentFailed    => "Failed",
            PaymentStatus.PaymentRefunded  => "Refunded",
            _                              => "Unknown",
        };
    }
}
