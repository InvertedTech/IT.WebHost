using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace Admin.Components.Pages.Settings.Personalization
{
    public partial class PersonalizationSettings
    {
        [Inject] SettingsClient SettingsClient { get; set; } = null!;
        [Inject] PublicSettingsClient PublicSettingsClient { get; set; } = null!;

        private PersonalizationPublicRecord _personalizationPublicSettings { get; set; } = new();

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
            // TODO: Any future owner/private personalization edits require adding AuthorizeView with appropriate roles (admin/owner) around save UI
        }
    }
}
