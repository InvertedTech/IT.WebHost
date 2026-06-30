using Core.Extensions;
using IT.WebServices.Authentication;
using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;
using ProtoValidate;

namespace Admin.Components.Pages.Content
{
    public partial class CreateContent
    {
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private ContentClient ContentClient { get; set; } = null!;
        [Inject] private IValidator Validator { get; set; } = null!;
        [Inject] private IToastService ToastService { get; set; } = null!;

        // TODO: Add role guard - CreateContent/Modify requires ROLE_CAN_CREATE_CONTENT, publish requires ROLE_CAN_PUBLISH; place AuthorizeView in .razor

        private async Task HandleFinish()
        {
            overallError = null;
            createdContentId = null;

            var req = BuildRequest();
            var validation = ValidateRequest(req);
            ApplyValidationErrors(validation);

            if (validation != null && !validation.IsSuccess)
            {
                StateHasChanged();
                return;
            }

            try
            {
                var res = await ContentClient.CreateContent(req);

                if (res?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } err)
                {
                    overallError = !string.IsNullOrEmpty(err.Message) ? err.Message : err.Reason.ToString();
                    ToastService.Error(overallError);
                    StateHasChanged();
                    return;
                }

                if (res?.Record?.Public?.ContentID is string id && !string.IsNullOrEmpty(id))
                {
                    createdContentId = id;
                }

                ToastService.Success("Content created successfully.");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                overallError = ex.Message;
                ToastService.Error(overallError);
                StateHasChanged();
            }
        }

        private CreateContentRequest BuildRequest()
        {
            var req = new CreateContentRequest()
            {
                Public = new()
                {
                    Title = title,
                    Description = description ?? "",
                    Author = authorId,
                    URL = "",
                    FeaturedImageAssetID = featuredImageAssetId,
                    SubscriptionLevel = subscriptionLevel,
                    AuthorID = authorId,
                },
                Private = new() { }
            };

            if (channels is not null && channels.Any())
            {
                req.Public.ChannelIds.AddRange(channels);
            }

            if (categories is not null && categories.Any())
            {
                req.Public.CategoryIds.AddRange(categories);
            }

            if (tags is not null && tags.Any())
            {
                req.Public.Tags.AddRange(tags);
            }

            switch (contentType)
            {
                case ContentType.ContentWritten:
                    req.Public.Written = new()
                    {
                        HtmlBody = htmlBody
                    };
                    req.Private.Written = new();
                    break;
                case ContentType.ContentAudio:
                    req.Public.Audio = new()
                    {
                        //AudioAssetID = audioAssetId
                        HtmlBody = htmlBody
                    };
                    req.Private.Audio = new();
                    break;
                case ContentType.ContentPicture:
                    req.Public.Picture = new()
                    {
                        HtmlBody = htmlBody,
                        //ImageAssetIDs = new()
                    };
                    req.Private.Picture = new();
                    break;
                case ContentType.ContentVideo:
                    req.Public.Video = new()
                    {
                        HtmlBody = htmlBody,
                        RumbleVideoId = rumbleVideoId ?? "",
                        YoutubeVideoId = youtubeVideoId ?? "",
                        IsLive = isLive,
                        IsLiveStream = isLiveStream,
                    };
                    req.Private.Video = new();
                    break;
                default:
                    break;
            }

            return req;
        }

        private IReadOnlyList<string> titleErrors = [];
        private IReadOnlyList<string> descriptionErrors = [];
        private IReadOnlyList<string> authorErrors = [];
        private IReadOnlyList<string> featuredImageErrors = [];
        private IReadOnlyList<string> channelErrors = [];
        private IReadOnlyList<string> categoryErrors = [];
        private IReadOnlyList<string> tagsErrors = [];
        private IReadOnlyList<string> subscriptionLevelErrors = [];
        private IReadOnlyList<string> htmlBodyErrors = [];
        private IReadOnlyList<string> rumbleVideoIdErrors = [];
        private IReadOnlyList<string> youtubeVideoIdErrors = [];

        private ValidationResult? lastValidation;
        private string? overallError;
        private string? createdContentId;

        private ValidationResult? ValidateRequest(CreateContentRequest req)
        {
            var validation = Validator.Validate(req, failFast: false);

            return validation;
        }

        private void ApplyValidationErrors(ValidationResult? validation)
        {
            if (validation is null)
            {
                ClearAllErrors();
                return;
            }

            lastValidation = validation;

            titleErrors = validation.Violations.ForField("Title").Errors;
            descriptionErrors = validation.Violations.ForField("Description").Errors;
            authorErrors = validation.Violations.ForField("Author").Errors;
            if (authorErrors.Count == 0)
            {
                authorErrors = validation.Violations.ForField("AuthorID").Errors;
            }
            featuredImageErrors = validation.Violations.ForField("FeaturedImageAssetID").Errors;
            channelErrors = validation.Violations.ForField("ChannelIds").Errors;
            categoryErrors = validation.Violations.ForField("CategoryIds").Errors;
            tagsErrors = validation.Violations.ForField("Tags").Errors;
            subscriptionLevelErrors = validation.Violations.ForField("SubscriptionLevel").Errors;
            htmlBodyErrors = validation.Violations.ForField("HtmlBody").Errors;
            rumbleVideoIdErrors = validation.Violations.ForField("RumbleVideoId").Errors;
            youtubeVideoIdErrors = validation.Violations.ForField("YoutubeVideoId").Errors;
        }

        private void ClearAllErrors()
        {
            titleErrors = [];
            descriptionErrors = [];
            authorErrors = [];
            featuredImageErrors = [];
            channelErrors = [];
            categoryErrors = [];
            tagsErrors = [];
            subscriptionLevelErrors = [];
            htmlBodyErrors = [];
            rumbleVideoIdErrors = [];
            youtubeVideoIdErrors = [];
            overallError = null;
            lastValidation = null;
        }

        private void ClearTitleErrors() => titleErrors = [];
        private void ClearDescriptionErrors() => descriptionErrors = [];
        private void ClearAuthorErrors() => authorErrors = [];
        private void ClearFeaturedImageErrors() => featuredImageErrors = [];
        private void ClearChannelErrors() => channelErrors = [];
        private void ClearCategoryErrors() => categoryErrors = [];
        private void ClearTagsErrors() => tagsErrors = [];
        private void ClearSubscriptionErrors() => subscriptionLevelErrors = [];
        private void ClearHtmlBodyErrors() => htmlBodyErrors = [];
        private void ClearRumbleErrors() => rumbleVideoIdErrors = [];
        private void ClearYoutubeErrors() => youtubeVideoIdErrors = [];

        private IReadOnlyList<string> GetAllValidationErrors()
        {
            var list = new List<string>();
            list.AddRange(titleErrors);
            list.AddRange(descriptionErrors);
            list.AddRange(authorErrors);
            list.AddRange(featuredImageErrors);
            list.AddRange(channelErrors);
            list.AddRange(categoryErrors);
            list.AddRange(tagsErrors);
            list.AddRange(subscriptionLevelErrors);
            list.AddRange(htmlBodyErrors);
            list.AddRange(rumbleVideoIdErrors);
            list.AddRange(youtubeVideoIdErrors);
            return list;
        }
    }

    public record CreateContentErrors(
            string? titleError,
            string? descriptionError,
            string? authorError,
            string? urlError,
            string? contentTypeError,
            string? channelError,
            string? categoryError,
            string? tagsError,
            string? featuredImageError,
            string? subscriptionLevelError,
            string? htmlBodyError,
            string? rumbleVideoIdError,
            string? youtubeVideoIdError,
            string? isLiveError,
            string? isLiveStreamError
        );
}
