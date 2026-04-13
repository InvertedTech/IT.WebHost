using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class CreateContent
    {
        [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;

        private ContentPublicData ContentPublicData { get; set; } = new();
        private ContentType Type { get; set; } = ContentType.ContentNone;
        private string SelectedChannelId { get; set; } = string.Empty;
        private string SelectedCategoryId { get; set; } = string.Empty;
        private IReadOnlyList<string> SelectedTags { get; set; } = new List<string>();
        private string UrlStub => GetStub();
        private void HandleComplete()
        {

        }

        private string GetStub()
        {
            switch (Type)
            {
                case ContentType.ContentNone:
                    return $"/{ContentPublicData.Title ?? string.Empty}";
                case ContentType.ContentAudio:
                    return $"/listen/{ContentPublicData.Title ?? string.Empty}";
                case ContentType.ContentPicture:
                    return $"/view/{ContentPublicData.Title ?? string.Empty}";
                case ContentType.ContentVideo:
                    return $"/watch/{ContentPublicData.Title ?? string.Empty}";
                case ContentType.ContentWritten:
                    return $"/read/{ContentPublicData.Title ?? string.Empty}";
                default:
                    return string.Empty;
            }
        }

        private void OnContentTypeSelected(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.ContentNone:
                    break;
                case ContentType.ContentAudio:
                    Type = ContentType.ContentAudio;
                    ContentPublicData.Audio = new();
                    break;
                case ContentType.ContentPicture:
                    Type = ContentType.ContentPicture;
                    ContentPublicData.Picture = new();
                    break;
                case ContentType.ContentWritten:
                    Type = ContentType.ContentWritten;
                    ContentPublicData.Written = new();
                    break;
                case ContentType.ContentVideo:
                    Type = ContentType.ContentVideo;
                    ContentPublicData.Video = new();
                    break;
                default:
                    break;
            }
        }

        private void OnHtmlBodyChanged(string htmlBody)
        {
            switch (Type)
            {
                case ContentType.ContentNone:
                    break;
                case ContentType.ContentAudio:
                    ContentPublicData.Audio.HtmlBody = htmlBody;
                    break;
                case ContentType.ContentPicture:
                    ContentPublicData.Picture.HtmlBody = htmlBody;
                    break;
                case ContentType.ContentWritten:
                    ContentPublicData.Written.HtmlBody = htmlBody;
                    break;
                case ContentType.ContentVideo:
                    ContentPublicData.Video.HtmlBody = htmlBody;
                    break;
            }
        }
    }
}
