using IT.WebServices.Authentication;
using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class CreateContent
    {
        [Inject] private ContentClient ContentClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private NavigationManager Nav { get; set; } = null!;

        private ContentPublicData ContentPublicData { get; set; } = new();
        private ContentPrivateData ContentPrivateData { get; set; } = new();
        private ContentType Type { get; set; } = ContentType.ContentNone;
        private string? _errorMessage;
        private string SelectedChannelId { get; set; } = string.Empty;
        private string SelectedCategoryId { get; set; } = string.Empty;
        private string SelectedAuthorId { get; set; } = string.Empty;
        private IReadOnlyList<string> SelectedTags { get; set; } = new List<string>();
        private IReadOnlyList<string> _pictureImageIds = [];
        private string UrlStub => GetStub();
        private async Task HandleComplete()
        {
            ContentPublicData.URL = UrlStub;
            ContentPublicData.AuthorID = SelectedAuthorId;

            if (!string.IsNullOrEmpty(SelectedChannelId))
                ContentPublicData.ChannelIds.Add(SelectedChannelId);

            if (!string.IsNullOrEmpty(SelectedCategoryId))
                ContentPublicData.CategoryIds.Add(SelectedCategoryId);

            ContentPublicData.Tags.AddRange(SelectedTags);

            _errorMessage = null;

            var res = await ContentClient.CreateContent(
                new CreateContentRequest { Public = ContentPublicData, Private = ContentPrivateData });

            if (res?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } err)
            {
                _errorMessage = string.IsNullOrEmpty(err.Message) ? err.Reason.ToString() : err.Message;
                return;
            }

            if (res?.Record?.Public?.ContentID is not null)
                Nav.NavigateTo("/admin/content");
        }

        private bool ValidateTypeStep() => Type != ContentType.ContentNone;

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

        private void OnPictureImagesChanged(IReadOnlyList<string> ids)
        {
            _pictureImageIds = ids;
            if (ContentPublicData.Picture != null)
            {
                ContentPublicData.Picture.ImageAssetIDs.Clear();
                ContentPublicData.Picture.ImageAssetIDs.AddRange(ids);
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
                    ContentPrivateData.Audio = new();
                    break;
                case ContentType.ContentPicture:
                    Type = ContentType.ContentPicture;
                    ContentPublicData.Picture = new();
                    ContentPrivateData.Picture = new();
                    break;
                case ContentType.ContentWritten:
                    Type = ContentType.ContentWritten;
                    ContentPublicData.Written = new();
                    ContentPrivateData.Written = new();
                    break;
                case ContentType.ContentVideo:
                    Type = ContentType.ContentVideo;
                    ContentPublicData.Video = new();
                    ContentPrivateData.Video = new();
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
