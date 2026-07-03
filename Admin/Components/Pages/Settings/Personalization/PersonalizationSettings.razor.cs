using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Settings.Personalization
{
    public partial class PersonalizationSettings
    {
        [Inject] SettingsClient SettingsClient { get; set; } = null!;
        [Inject] PublicSettingsClient PublicSettingsClient { get; set; } = null!;
        [Inject] IToastService ToastService { get; set; } = null!;

        private PersonalizationPublicRecord _personalizationPublicSettings { get; set; } = new();
        private bool IsEditing { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            StateHasChanged();
        }

        private async Task LoadSettings()
        {
            var res = await PublicSettingsClient.PublicData;

            if (res is not  null)
            {
                _personalizationPublicSettings = res.Personalization;
            }
        }

        private void StartEdit() => IsEditing = true;

        private async Task CancelEdit()
        {
            IsEditing = false;
            await LoadSettings();
        }

        private async Task SaveSettingsAsync()
        {
            var req = new ModifyPersonalizationPublicDataRequest
            {
                Data = _personalizationPublicSettings
            };

            var res = await SettingsClient.ModifyPersonalizationPublicSettings(req);

            if (res is null || res.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("Personalization settings saved successfully.");
                IsEditing = false;
                await LoadSettings();
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Message)
                    ? res.Message
                    : res.Reason.ToString();

                ToastService.Error(message);
            }
        }
    }
}
