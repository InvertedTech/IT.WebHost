using Google.Protobuf.WellKnownTypes;
using Google.Protobuf.WellKnownTypes;
using IT.WebServices.Authentication;
using IT.WebServices.Clients.CMS;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class ModifyContent
    {
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private ContentClient ContentClient { get; set; } = null!;

        [Parameter] public string ContentId { get; set; } = null!;

        private ContentPublicRecord? Public { get; set; }
        private ContentPrivateRecord? Private { get; set; }
        private bool _isLoading = true;
        private bool _isEditing = false;
        private IReadOnlyList<string> _editTags = [];
        private DateTime? _publishDate;
        private string _htmlBody = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            await LoadContent();
            _isLoading = false;
        }

        private async Task LoadContent()
        {
            var req = new GetContentAdminRequest()
            {
                ContentID = ContentId,
            };

            var res = await ContentClient.GetContentAdmin(req);

            if (res != null)
            {
                Public = res.Record.Public;
                Private = res.Record.Private;
                _editTags = Public?.Data?.Tags?.ToList() ?? [];
                _htmlBody = Public?.Data?.Written?.HtmlBody ?? string.Empty;
                var raw = Public?.PublishOnUTC?.ToDateTime();
                _publishDate = raw is { } dt && dt != default ? dt.ToLocalTime() : null;
            }
        }

        private void OnHtmlBodyChanged(string value)
        {
            _htmlBody = value;
            if (Public?.Data?.Written != null)
                Public.Data.Written.HtmlBody = value;
        }

        private void StartEdit() => _isEditing = true;

        private async Task CancelEdit()
        {
            _isEditing = false;
            await LoadContent();
        }

        private async Task SaveContent()
        {
            if (Public?.Data == null) return;

            Public.Data.Tags.Clear();
            Public.Data.Tags.AddRange(_editTags);
            Public.PublishOnUTC = _publishDate.HasValue
                ? Timestamp.FromDateTime(_publishDate.Value.ToUniversalTime())
                : null;

            var modifyRes = await ContentClient.ModifyContent(new ModifyContentRequest
            {
                ContentID = Public.ContentID,
                Public    = Public.Data,
                Private   = Private?.Data ?? new(),
            });

            if (modifyRes?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } err)
            {
                Console.WriteLine($"Save failed: {err.Message}");
                return;
            }

            await ContentClient.PublishContent(new PublishContentRequest
            {
                ContentID   = Public.ContentID,
                PublishOnUTC = _publishDate.HasValue
                    ? Timestamp.FromDateTime(_publishDate.Value.ToUniversalTime())
                    : null,
            });

            _isEditing = false;
            await LoadContent();
        }
    }
}
