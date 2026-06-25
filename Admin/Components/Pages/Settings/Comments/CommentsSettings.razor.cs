using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Comment;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Settings.Comments
{
    public partial class CommentsSettings
    {
        [Inject] public PublicSettingsClient PublicSettingsClient { get; set; } = null!;
        [Inject] public SettingsClient SettingsClient { get; set; } = null!;
        [Inject] public IToastService ToastService { get; set; } = null!;

        private bool isLoading { get; set; } = true;

        private CommentsPublicRecord _publicSettings { get; set; } = new()
        {
            DefaultRestriction = new()
        };

        private List<string> _blackList { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            // TODO: Page requires "owner" (or service) role due to SettingsClient.PrivateData fetch (GetOwnerData); public mods admin/owner. Add AuthorizeView in .razor for the whole UI or sections.
            await LoadSettings();
        }

        private async Task LoadSettings()
        {
            isLoading = true;
            await Task.WhenAll(LoadPublicSettings(), LoadPrivateSettings());
            isLoading = false;
        }

        private async Task LoadPublicSettings()
        {
            var res = await PublicSettingsClient.PublicData;

            if (res?.Comments is not null)
            {
                _publicSettings = res.Comments;
                _publicSettings.DefaultRestriction ??= new CommentRestrictionMinimum();
            }
        }

        private async Task LoadPrivateSettings()
        {
            var res = await SettingsClient.PrivateData;

            if (res?.Comments?.BlackList is not null)
            {
                _blackList = res.Comments.BlackList.ToList();
            }
        }

        private async Task SavePublicSettings()
        {
            _publicSettings.DefaultRestriction ??= new CommentRestrictionMinimum();

            var error = await SettingsClient.ModifyCommentsPublicSettings(new ModifyCommentsPublicDataRequest
            {
                Data = _publicSettings.Clone()
            });

            if (error is null || error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("Comment settings saved successfully.");
            }
            else
            {
                var message = !string.IsNullOrEmpty(error.Message)
                    ? error.Message
                    : error.Reason.ToString();

                ToastService.Error(message);
            }
        }

        private async Task SavePrivateSettings()
        {
            var record = new CommentsPrivateRecord();
            record.BlackList.AddRange(_blackList.Where(term => !string.IsNullOrWhiteSpace(term)));

            var error = await SettingsClient.ModifyCommentsPrivateSettings(new ModifyCommentsPrivateDataRequest
            {
                Data = record
            });

            if (error is null || error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("Blacklist saved successfully.");
            }
            else
            {
                var message = !string.IsNullOrEmpty(error.Message)
                    ? error.Message
                    : error.Reason.ToString();

                ToastService.Error(message);
            }
        }

        private void AddBlackListEntry()
        {
            _blackList.Add("");
        }

        private void RemoveBlackListEntry(int index)
        {
            if (index >= 0 && index < _blackList.Count)
            {
                _blackList.RemoveAt(index);
            }
        }
    }
}