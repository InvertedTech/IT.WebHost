using IT.WebServices.Fragments.Authentication;

namespace Admin.Models
{
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
            UserId = record.Public?.UserID ?? string.Empty,
            DisplayName = record.Public?.Data?.DisplayName ?? string.Empty,
            UserName = record.Public?.Data?.UserName ?? string.Empty,
            Bio = record.Public?.Data?.Bio ?? string.Empty,
            Email = record.Private?.Data?.Email ?? string.Empty,
            FirstName = record.Private?.Data?.FirstName ?? string.Empty,
            LastName = record.Private?.Data?.LastName ?? string.Empty,
            PostalCode = record.Private?.Data?.PostalCode ?? string.Empty,
            ProfileImagePNG = record.Public?.Data?.ProfileImagePNG?.ToByteArray() ?? [],
            Roles = record.Private?.Roles?.ToList() ?? [],
            CreatedOnUTC = record.Public?.CreatedOnUTC?.ToDateTime(),
        };
    }
}