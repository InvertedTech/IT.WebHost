using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;

namespace IT.WebHost.Core.Models
{
    public class ProfileViewModel
    {
        public ProfileData? UserRecord { get; set; }
        public IEnumerable<GenericSubscriptionRecord> SubscriptionRecords { get; set; } = new List<GenericSubscriptionRecord>();
    }

    public class ProfileData
    {
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public byte[] ProfileImagePNG { get; set; } = [];
        public List<string> Roles { get; set; } = [];
        public DateTime? CreatedOnUTC { get; set; }
        

        public static ProfileData FromRecord(UserNormalRecord record) => new()
        {
            DisplayName    = record.Public?.Data?.DisplayName ?? string.Empty,
            UserName       = record.Public?.Data?.UserName    ?? string.Empty,
            Bio            = record.Public?.Data?.Bio         ?? string.Empty,
            ProfileImagePNG = record.Public?.Data?.ProfileImagePNG?.ToByteArray() ?? [],
            Roles          = record.Private?.Roles?.ToList() ?? [],
            CreatedOnUTC   = record.Public?.CreatedOnUTC?.ToDateTime(),
        };
    }
}
