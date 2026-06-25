using Admin.Models;
using Google.Protobuf.WellKnownTypes;
using IT.WebHost.Core.Config;
using IT.WebServices.Authentication;
using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using NeoUI.Blazor;

// TODO: Bind Methods
namespace Admin.Components.Pages.Content
{
    public partial class ViewContent
    {
        [Parameter]
        public string Id { get; set; } = null!;

        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private ContentClient ContentClient { get; set; } = null!;
        [Inject] private IToastService ToastService { get; set; } = null!;
        [Inject] private IOptions<AppSettings> _settings { get; set; } = null!;

        private ContentRecord? Content { get; set; }
        private ContentPrivateData? PrivateData { get; set; }
        private ContentEditData? EditContent { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsEditing { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;
            IsEditing = false;
            Content = null;
            // TODO: Load (GetContentAdmin) and Modify require ROLE_CAN_CREATE_CONTENT; PublishContent requires ROLE_CAN_PUBLISH. Add AuthorizeView(s) in .razor accordingly for buttons/editing UI.
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
            IsLoading = false;
        }

        private async Task LoadContent()
        {
            var res = await ContentClient.GetContentAdmin(new GetContentAdminRequest
            {
                ContentID = Id,
            });

            if (res?.Record is not null)
            {
                Content = res.Record;
                PrivateData = res.Record.Private?.Data;
            }
        }

        private void StartEdit() => IsEditing = true;

        private async Task CancelEdit()
        {
            IsEditing = false;
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
        }

        private async Task SaveContent()
        {
            if (EditContent is null)
            {
                return;
            }

            var modifyRes = await ContentClient.ModifyContent(new ModifyContentRequest
            {
                ContentID = EditContent.ContentId,
                Public = EditContent.ToPublicData(),
                Private = PrivateData ?? new ContentPrivateData(),
            });

            if (modifyRes?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } err)
            {
                var message = !string.IsNullOrEmpty(err.Message)
                    ? err.Message
                    : err.Reason.ToString();

                ToastService.Error(message);
                return;
            }


            ToastService.Success("Content updated successfully.");
            IsEditing = false;
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
        }

        private async Task HandlePublish(Timestamp publishOnUTC)
        {
            var req = new PublishContentRequest
            {
                ContentID = EditContent!.ContentId,
                PublishOnUTC = publishOnUTC,
            };

            var publishRes = await ContentClient.PublishContent(req);

            if (publishRes?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } publishErr)
            {
                var message = !string.IsNullOrEmpty(publishErr.Message)
                    ? publishErr.Message
                    : publishErr.Reason.ToString();

                ToastService.Error(message);
                return;
            }

            ToastService.Success("Content published successfully.");
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
            StateHasChanged();
        }

        private async Task HandleUnpublish()
        {
            var req = new UnpublishContentRequest()
            {
                ContentID = Content.Public.ContentID
            };

            // TODO: Handle Res
            var res = await ContentClient.UnpublishContent(req);

            ToastService.Success("Content unpublished successfully.");
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
            StateHasChanged();
        }

        private async Task HandleDelete()
        {
            var req = new DeleteContentRequest()
            {
                ContentID = Content.Public.ContentID
            };

            var res = await ContentClient.DeleteContent(req);
            // TODO: Handle Res
            ToastService.Success("Content deleted successfully.");
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
            StateHasChanged();
        }

        private async Task HandleUnDelete()
        {
            var req = new UndeleteContentRequest()
            {
                ContentID = Content.Public.ContentID
            };

            var res = await ContentClient.UnDeleteContent(req);
            // TODO: Client Call
            ToastService.Success("Content restored successfully.");
            await LoadContent();
            EditContent = Content is not null ? ContentEditData.FromRecord(Content) : null;
            StateHasChanged();
        }

        private string GetAssetUrl(string assetId) =>
            $"{_settings.Value.API_BASE_URL}/cms/asset/{assetId}/data";

        private static string GetTypeLabel(ContentType type) => type switch
        {
            ContentType.ContentVideo => "Video",
            ContentType.ContentWritten => "Written",
            ContentType.ContentPicture => "Picture",
            ContentType.ContentAudio => "Audio",
            _ => "Unknown",
        };

        private static string GetTypeIcon(ContentType type) => type switch
        {
            ContentType.ContentVideo => "video",
            ContentType.ContentWritten => "newspaper",
            ContentType.ContentPicture => "image",
            ContentType.ContentAudio => "headphones",
            _ => "file",
        };
    }
}