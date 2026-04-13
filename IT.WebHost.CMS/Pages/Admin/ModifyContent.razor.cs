using Google.Protobuf.WellKnownTypes;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin
{
    public partial class ModifyContent
    {
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;

        [Parameter] public string ContentId { get; set; } = null!;

        private ContentPublicRecord? Public { get; set; }
        private ContentPrivateRecord? Private { get; set; }
        private bool _isLoading = true;
        private bool _isEditing = false;
        private IReadOnlyList<string> _editTags = [];
        private DateTime? _publishDate;

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

            var res = await ContentClient.GetContentAdminAsync(req, UserHelper.GetGrpcCallOptions());

            if (res != null)
            {
                Public = res.Record.Public;
                Private = res.Record.Private;
                _editTags = Public?.Data?.Tags?.ToList() ?? [];
                var raw = Public?.PublishOnUTC?.ToDateTime();
                _publishDate = raw is { } dt && dt != default ? dt.ToLocalTime() : null;
            }
        }

        private void StartEdit() => _isEditing = true;

        private async Task CancelEdit()
        {
            _isEditing = false;
            await LoadContent();
        }

        private Task SaveContent()
        {
            if (Public?.Data != null)
            {
                Public.Data.Tags.Clear();
                Public.Data.Tags.AddRange(_editTags.ToList());
            }
            if (Public != null)
            {
                Public.PublishOnUTC = _publishDate.HasValue
                    ? Timestamp.FromDateTime(_publishDate.Value.ToUniversalTime())
                    : null;
            }
            _isEditing = false;
            // TODO: wire up gRPC save call
            return Task.CompletedTask;
        }
    }
}
