using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Content;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Settings.CMS
{
    public partial class CMSSettings
    {
        [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
        [Inject] private SettingsClient SettingsClient { get; set; } = null!;

        private CMSPublicRecord _cms = new();
        private LayoutEnum _selectedLayout = LayoutEnum.List;

        // Channel sheet
        private bool _channelSheetOpen = false;
        private ChannelRecord? _editingChannel = null;
        private string _newChannelName = "";
        private string _newChannelSlug = "";
        private string _newChannelParentId = "";
        private string _newChannelYoutubeUrl = "";
        private string _newChannelRumbleUrl = "";
        private string? _channelError;

        // Category sheet
        private bool _categorySheetOpen = false;
        private CategoryRecord? _editingCategory = null;
        private string _newCategoryName = "";
        private string _newCategorySlug = "";
        private string _newCategoryParentId = "";
        private string? _categoryError;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var pub = await PublicSettingsClient.PublicData;
            _cms = pub.CMS?.Clone() ?? new CMSPublicRecord();
            _selectedLayout = _cms.DefaultLayout;
        }

        private void OpenChannelSheet()
        {
            _editingChannel = null;
            ResetChannelForm();
            _channelSheetOpen = true;
        }

        private void OpenChannelEditSheet(ChannelRecord ch)
        {
            _editingChannel = ch;
            _newChannelName = ch.DisplayName;
            _newChannelSlug = ch.UrlStub;
            _newChannelParentId = ch.ParentChannelId;
            _newChannelYoutubeUrl = ch.YoutubeUrl;
            _newChannelRumbleUrl = ch.RumbleUrl;
            _channelError = null;
            _channelSheetOpen = true;
        }

        private void OpenCategorySheet()
        {
            _editingCategory = null;
            ResetCategoryForm();
            _categorySheetOpen = true;
        }

        private void OpenCategoryEditSheet(CategoryRecord cat)
        {
            _editingCategory = cat;
            _newCategoryName = cat.DisplayName;
            _newCategorySlug = cat.UrlStub;
            _newCategoryParentId = cat.ParentCategoryId;
            _categoryError = null;
            _categorySheetOpen = true;
        }

        private async Task SaveLayoutAsync()
        {
            var record = _cms.Clone();
            record.DefaultLayout = _selectedLayout;
            await SettingsClient.ModifyCMSPublicSettings(new ModifyCMSPublicDataRequest { Data = record });
            PublicSettingsClient.InvalidateCache();
            await LoadAsync();
        }

        private async Task SaveChannelAsync()
        {
            _channelError = null;
            _newChannelName = _newChannelName.Trim();
            _newChannelSlug = _newChannelSlug.Trim();

            if (string.IsNullOrEmpty(_newChannelName) || string.IsNullOrEmpty(_newChannelSlug))
            {
                _channelError = "Display Name and URL Stub are required.";
                return;
            }

            var record = _cms.Clone();

            if (_editingChannel != null)
            {
                var existing = record.Channels.FirstOrDefault(c => c.ChannelId == _editingChannel.ChannelId);
                if (existing != null)
                {
                    existing.DisplayName = _newChannelName;
                    existing.UrlStub = _newChannelSlug;
                    existing.ParentChannelId = _newChannelParentId;
                    existing.YoutubeUrl = _newChannelYoutubeUrl;
                    existing.RumbleUrl = _newChannelRumbleUrl;
                }
            }
            else
            {
                record.Channels.Add(new ChannelRecord
                {
                    ChannelId = Guid.NewGuid().ToString(),
                    DisplayName = _newChannelName,
                    UrlStub = _newChannelSlug,
                    ParentChannelId = _newChannelParentId,
                    YoutubeUrl = _newChannelYoutubeUrl,
                    RumbleUrl = _newChannelRumbleUrl,
                });
            }

            await SettingsClient.ModifyCMSPublicSettings(new ModifyCMSPublicDataRequest { Data = record });
            PublicSettingsClient.InvalidateCache();
            _channelSheetOpen = false;
            ResetChannelForm();
            await LoadAsync();
        }

        private async Task SaveCategoryAsync()
        {
            _categoryError = null;
            _newCategoryName = _newCategoryName.Trim();
            _newCategorySlug = _newCategorySlug.Trim();

            if (string.IsNullOrEmpty(_newCategoryName) || string.IsNullOrEmpty(_newCategorySlug))
            {
                _categoryError = "Display Name and URL Stub are required.";
                return;
            }

            var record = _cms.Clone();

            if (_editingCategory != null)
            {
                var existing = record.Categories.FirstOrDefault(c => c.CategoryId == _editingCategory.CategoryId);
                if (existing != null)
                {
                    existing.DisplayName = _newCategoryName;
                    existing.UrlStub = _newCategorySlug;
                    existing.ParentCategoryId = _newCategoryParentId;
                }
            }
            else
            {
                record.Categories.Add(new CategoryRecord
                {
                    CategoryId = Guid.NewGuid().ToString(),
                    DisplayName = _newCategoryName,
                    UrlStub = _newCategorySlug,
                    ParentCategoryId = _newCategoryParentId,
                });
            }

            await SettingsClient.ModifyCMSPublicSettings(new ModifyCMSPublicDataRequest { Data = record });
            PublicSettingsClient.InvalidateCache();
            _categorySheetOpen = false;
            ResetCategoryForm();
            await LoadAsync();
        }

        private void ResetChannelForm()
        {
            _newChannelName = "";
            _newChannelSlug = "";
            _newChannelParentId = "";
            _newChannelYoutubeUrl = "";
            _newChannelRumbleUrl = "";
            _channelError = null;
        }

        private void ResetCategoryForm()
        {
            _newCategoryName = "";
            _newCategorySlug = "";
            _newCategoryParentId = "";
            _categoryError = null;
        }
    }
}
