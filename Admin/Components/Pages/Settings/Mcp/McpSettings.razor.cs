using IT.WebServices.Authentication;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Settings.Mcp
{
    public partial class McpSettings
    {
        [Inject] SettingsClient SettingsClient { get; set; } = null!;
        [Inject] IToastService ToastService { get; set; } = null!;
        [Inject] ONUserHelper UserHelper { get; set; } = null!;

        private bool isLoading = true;
        private bool isSaving;

        private bool IsOwner => UserHelper.MyUser?.RoleAbilities.IsOwner == true;

        private McpPrivateRecord _private = Normalize(null);
        private McpOwnerRecord _owner = new();
        private List<string> _origins = new();

        private int ReadPerMinute
        {
            get => (int)_private.Limits.ReadRequestsPerMinute;
            set => _private.Limits.ReadRequestsPerMinute = (uint)Math.Max(0, value);
        }

        private int WritePerMinute
        {
            get => (int)_private.Limits.WriteRequestsPerMinute;
            set => _private.Limits.WriteRequestsPerMinute = (uint)Math.Max(0, value);
        }

        private int WritePerDay
        {
            get => (int)_private.Limits.WriteRequestsPerDay;
            set => _private.Limits.WriteRequestsPerDay = (uint)Math.Max(0, value);
        }

        private int MaxBodyKb
        {
            get => (int)(_private.Limits.MaxBodyBytes / 1024);
            set => _private.Limits.MaxBodyBytes = (uint)Math.Max(0, value) * 1024;
        }

        private int MaxNodes
        {
            get => (int)_private.Pages.MaxNodes;
            set => _private.Pages.MaxNodes = (uint)Math.Max(0, value);
        }

        private int MaxDepth
        {
            get => (int)_private.Pages.MaxDepth;
            set => _private.Pages.MaxDepth = (uint)Math.Max(0, value);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
        }

        private static McpPrivateRecord Normalize(McpPrivateRecord? record)
        {
            var copy = record?.Clone() ?? new McpPrivateRecord();
            copy.Limits ??= new();
            copy.Pages ??= new();
            copy.Cms ??= new();
            copy.Comments ??= new();
            return copy;
        }

        private async Task LoadSettings()
        {
            isLoading = true;

            var privateData = await SettingsClient.PrivateData;
            _private = Normalize(privateData?.MCP);
            _origins = _private.AllowedOrigins.ToList();

            if (IsOwner)
            {
                var ownerData = await SettingsClient.OwnerData;
                _owner = ownerData?.MCP?.Clone() ?? new McpOwnerRecord();
            }

            isLoading = false;
        }

        private async Task SavePrivate()
        {
            var origins = _origins
                .Select(o => o.Trim().TrimEnd('/'))
                .Where(o => o.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var invalid = origins.FirstOrDefault(o =>
                !Uri.TryCreate(o, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps));

            if (invalid is not null)
            {
                ToastService.Error($"\"{invalid}\" is not a valid origin. Use a full address such as https://app.example.com.");
                return;
            }

            var record = _private.Clone();
            record.AllowedOrigins.Clear();
            record.AllowedOrigins.AddRange(origins);

            isSaving = true;
            var error = await SettingsClient.ModifyMCPPrivateSettings(new ModifyMCPPrivateDataRequest
            {
                Data = record
            });

            if (Report(error, "MCP settings saved successfully."))
                await LoadSettings();

            isSaving = false;
        }

        private async Task SaveOwner()
        {
            isSaving = true;
            var error = await SettingsClient.ModifyMCPOwnerSettings(new ModifyMCPOwnerDataRequest
            {
                Data = _owner.Clone()
            });

            if (Report(error, "Owner approval settings saved successfully."))
                await LoadSettings();

            isSaving = false;
        }

        private bool Report(APIError? error, string successMessage)
        {
            if (error is null || error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success(successMessage);
                return true;
            }

            var message = !string.IsNullOrEmpty(error.Message)
                ? error.Message
                : error.Reason.ToString();

            ToastService.Error(message);
            return false;
        }

        private void AddOrigin()
        {
            _origins.Add("");
        }

        private void RemoveOrigin(int index)
        {
            if (index >= 0 && index < _origins.Count)
                _origins.RemoveAt(index);
        }
    }
}
