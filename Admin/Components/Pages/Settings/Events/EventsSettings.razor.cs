using IT.WebServices.Authentication;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authorization.Events;
using IT.WebServices.Fragments.Authorization.Events.Eventbrite;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Settings.Events
{
    public partial class EventsSettings
    {
        [Inject] public PublicSettingsClient PublicSettingsClient { get; set; } = null!;
        [Inject] public SettingsClient SettingsClient { get; set; } = null!;
        [Inject] public IToastService ToastService { get; set; } = null!;
        [Inject] public ONUserHelper UserHelper { get; set; } = null!;

        private enum VenueType { Physical, Virtual }

        private bool isLoading { get; set; } = true;

        private bool IsOwner => UserHelper.MyUser?.RoleAbilities.IsOwner ?? false;

        private EventPublicSettings _publicSettings { get; set; } = new() { Eventbrite = new() };
        private List<GenericEventVenueRecord> _venues { get; set; } = new();
        private EventOwnerSettings _ownerSettings { get; set; } = new() { Eventbrite = new() };

        private bool _isSheetOpen = false;
        private GenericEventVenueRecord? _selectedVenue = null;
        private VenueType _editVenueType = VenueType.Physical;
        private GenericPhysicalEventVenue _editPhysical = new();
        private GenericVirtualEventVenue _editVirtual = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
        }

        private async Task LoadSettings()
        {
            isLoading = true;

            var tasks = new List<Task> { LoadPublicSettings(), LoadPrivateSettings() };
            if (IsOwner)
                tasks.Add(LoadOwnerSettings());

            await Task.WhenAll(tasks);

            isLoading = false;
        }

        private async Task LoadPublicSettings()
        {
            var res = await PublicSettingsClient.PublicData;

            if (res?.Events is not null)
            {
                _publicSettings = res.Events;
                _publicSettings.Eventbrite ??= new EventbriteEventPublicSettings();
            }
        }

        private async Task LoadPrivateSettings()
        {
            var res = await SettingsClient.PrivateData;

            if (res?.Events?.Venues is not null)
                _venues = res.Events.Venues.ToList();
        }

        private async Task LoadOwnerSettings()
        {
            var res = await SettingsClient.OwnerData;

            if (res?.Events is not null)
            {
                _ownerSettings = res.Events;
                _ownerSettings.Eventbrite ??= new EventbriteEventOwnerSettings();
            }
        }

        private async Task SavePublicSettings()
        {
            var error = await SettingsClient.ModifyEventPublicSettings(new ModifyEventPublicSettingsRequest
            {
                Data = _publicSettings.Clone()
            });

            ReportResult(error, "Event settings saved successfully.");
        }

        private async Task SaveVenues()
        {
            var data = new EventPrivateSettings();
            data.Venues.AddRange(_venues);

            var error = await SettingsClient.ModifyEventPrivateSettings(new ModifyEventPrivateSettingsRequest
            {
                Data = data
            });

            ReportResult(error, "Venues saved successfully.");
        }

        private async Task SaveOwnerSettings()
        {
            var error = await SettingsClient.ModifyEventOwnerSettings(new ModifyEventOwnerSettingsRequest
            {
                Data = _ownerSettings.Clone()
            });

            ReportResult(error, "Eventbrite credentials saved successfully.");
        }

        private void ReportResult(APIError error, string successMessage)
        {
            if (error is null || error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success(successMessage);
            }
            else
            {
                var message = !string.IsNullOrEmpty(error.Message) ? error.Message : error.Reason.ToString();
                ToastService.Error(message);
            }
        }

        private void OpenCreateVenueSheet()
        {
            _selectedVenue = null;
            _editVenueType = VenueType.Physical;
            _editPhysical = new();
            _editVirtual = new();
            _isSheetOpen = true;
        }

        private void OpenEditVenueSheet(GenericEventVenueRecord venue)
        {
            _selectedVenue = venue;

            if (venue.VenueOneOfCase == GenericEventVenueRecord.VenueOneOfOneofCase.Virtual)
            {
                _editVenueType = VenueType.Virtual;
                _editVirtual = venue.Virtual.Clone();
                _editPhysical = new();
            }
            else
            {
                _editVenueType = VenueType.Physical;
                _editPhysical = venue.Physical?.Clone() ?? new();
                _editVirtual = new();
            }

            _isSheetOpen = true;
        }

        private void CloseVenueSheet()
        {
            _selectedVenue = null;
            _isSheetOpen = false;
        }

        private void SaveVenueToList()
        {
            var venue = _selectedVenue ?? new GenericEventVenueRecord { VenueID = Guid.NewGuid().ToString() };

            if (_editVenueType == VenueType.Physical)
                venue.Physical = _editPhysical;
            else
                venue.Virtual = _editVirtual;

            if (_selectedVenue is null)
                _venues.Add(venue);

            CloseVenueSheet();
        }

        private void RemoveVenue(GenericEventVenueRecord venue) => _venues.Remove(venue);

        private static string VenueDisplayName(GenericEventVenueRecord venue) =>
            venue.VenueOneOfCase == GenericEventVenueRecord.VenueOneOfOneofCase.Virtual
                ? (string.IsNullOrEmpty(venue.Virtual?.Name) ? "(Unnamed Virtual Venue)" : venue.Virtual.Name)
                : (string.IsNullOrEmpty(venue.Physical?.Name) ? "(Unnamed Venue)" : venue.Physical.Name);

        private static string VenueDisplaySubtitle(GenericEventVenueRecord venue) =>
            venue.VenueOneOfCase == GenericEventVenueRecord.VenueOneOfOneofCase.Virtual
                ? venue.Virtual?.Url ?? ""
                : string.Join(", ", new[] { venue.Physical?.City, venue.Physical?.StateOrProvince }.Where(s => !string.IsNullOrEmpty(s)));
    }
}
