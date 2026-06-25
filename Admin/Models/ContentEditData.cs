using Google.Protobuf.WellKnownTypes;
using IT.WebServices.Fragments.Content;

namespace Admin.Models
{
    public class ContentEditData
    {
        public string ContentId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string FeaturedImageAssetId { get; set; } = string.Empty;
        public uint SubscriptionLevel { get; set; }
        public List<string> ChannelIds { get; set; } = [];
        public List<string> CategoryIds { get; set; } = [];
        public List<string> Tags { get; set; } = [];
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public DateOnly? PublishDate { get; set; }
        public ContentType ContentType { get; set; } = ContentType.ContentNone;

        public string HtmlBody { get; set; } = string.Empty;
        public string YoutubeVideoId { get; set; } = string.Empty;
        public string RumbleVideoId { get; set; } = string.Empty;
        public bool IsLiveStream { get; set; }
        public string AudioAssetId { get; set; } = string.Empty;
        public List<string> ImageAssetIds { get; set; } = [];

        public static ContentEditData FromRecord(ContentRecord record)
        {
            var pub = record.Public;
            var data = pub.Data;
            var edit = new ContentEditData
            {
                ContentId = pub.ContentID,
                Title = data.Title,
                Description = data.Description,
                Author = data.Author,
                AuthorId = data.AuthorID,
                FeaturedImageAssetId = data.FeaturedImageAssetID,
                SubscriptionLevel = data.SubscriptionLevel,
                ChannelIds = data.ChannelIds.ToList(),
                CategoryIds = data.CategoryIds.ToList(),
                Tags = data.Tags.ToList(),
                CreatedOnUtc = pub.CreatedOnUTC?.ToDateTime(),
                DeletedOnUtc = pub.DeletedOnUTC?.Seconds > 0 ? pub.DeletedOnUTC.ToDateTime() : null,
                ContentType = data.GetContentType(),
            };

            var rawPublish = pub.PublishOnUTC?.ToDateTime();
            if (rawPublish is { } dt && dt != default)
            {
                edit.PublishDate = DateOnly.FromDateTime(dt.ToLocalTime());
            }

            switch (data.ContentDataOneofCase)
            {
                case ContentPublicData.ContentDataOneofOneofCase.Written:
                    edit.HtmlBody = data.Written.HtmlBody;
                    break;
                case ContentPublicData.ContentDataOneofOneofCase.Video:
                    edit.HtmlBody = data.Video.HtmlBody;
                    edit.YoutubeVideoId = data.Video.YoutubeVideoId;
                    edit.RumbleVideoId = data.Video.RumbleVideoId;
                    edit.IsLiveStream = data.Video.IsLiveStream;
                    break;
                case ContentPublicData.ContentDataOneofOneofCase.Audio:
                    edit.HtmlBody = data.Audio.HtmlBody;
                    edit.AudioAssetId = data.Audio.AudioAssetID;
                    break;
                case ContentPublicData.ContentDataOneofOneofCase.Picture:
                    edit.HtmlBody = data.Picture.HtmlBody;
                    edit.ImageAssetIds = data.Picture.ImageAssetIDs.ToList();
                    break;
            }

            return edit;
        }

        public ContentPublicData ToPublicData()
        {
            var data = new ContentPublicData
            {
                Title = Title,
                Description = Description,
                Author = Author,
                AuthorID = AuthorId,
                FeaturedImageAssetID = FeaturedImageAssetId,
                SubscriptionLevel = SubscriptionLevel,
            };

            data.ChannelIds.AddRange(ChannelIds);
            data.CategoryIds.AddRange(CategoryIds);
            data.Tags.AddRange(Tags);

            switch (ContentType)
            {
                case ContentType.ContentWritten:
                    data.Written = new WrittenContentPublicData { HtmlBody = HtmlBody };
                    break;
                case ContentType.ContentVideo:
                    data.Video = new VideoContentPublicData
                    {
                        HtmlBody = HtmlBody,
                        YoutubeVideoId = YoutubeVideoId,
                        RumbleVideoId = RumbleVideoId,
                        IsLiveStream = IsLiveStream,
                    };
                    break;
                case ContentType.ContentAudio:
                    data.Audio = new AudioContentPublicData
                    {
                        HtmlBody = HtmlBody,
                        AudioAssetID = AudioAssetId,
                    };
                    break;
                case ContentType.ContentPicture:
                    data.Picture = new PictureContentPublicData
                    {
                        HtmlBody = HtmlBody,
                    };
                    data.Picture.ImageAssetIDs.AddRange(ImageAssetIds);
                    break;
            }

            return data;
        }

        public Timestamp? ToPublishTimestamp() =>
            PublishDate.HasValue
                ? Timestamp.FromDateTime(PublishDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime())
                : null;
    }
}