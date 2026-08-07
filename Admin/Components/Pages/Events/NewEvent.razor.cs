using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Authorization.Events;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Events
{
    public partial class NewEvent
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private AdminEventInterface.AdminEventInterfaceClient AdminEventClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private IToastService ToastService { get; set; } = null!;
        [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
        [Inject] private SettingsClient SettingsClient { get; set; } = null!;

        private bool _isLoading = true;
        private bool _eventbriteEnabled;
        private List<GenericEventVenueRecord> _venues = new();

        private string _title = "";
        private string _description = "";
        private string _location = "";
        private string _venueId = "";
        private uint _maxTickets = 0;
        private string _internalNotes = "";
        private bool _syncToEventbrite = false;
        private List<string> _tags = new();

        private DateOnly? _startDate;
        private int _startHour = DateTime.Now.Hour;
        private int _startMinute = 0;
        private DateOnly? _endDate;
        private int _endHour = DateTime.Now.Hour;
        private int _endMinute = 0;

        private List<GenericTicketClassRecord> _ticketClasses = new();

        private bool _isSheetOpen = false;
        private GenericTicketClassRecord? _selectedTicketClass = null;
        private string _editClassName = "";
        private GenericEventTicketClassType _editClassType = GenericEventTicketClassType.TicketGeneralAccess;
        private uint _editAmountAvailable = 0;
        private uint _editPricePerTicketCents = 0;
        private uint _editMaxTicketsPerUser = 0;
        private bool _editIsTransferrable = false;
        private bool _editCountTowardEventMax = true;
        private uint _editRequiredSubscriptionAmountCents = 0;

        private DateOnly? _editSaleStartDate;
        private int _editSaleStartHour = 0;
        private int _editSaleStartMinute = 0;
        private DateOnly? _editSaleEndDate;
        private int _editSaleEndHour = 0;
        private int _editSaleEndMinute = 0;

        private bool IsSaving { get; set; }
        private string? OverallError { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var publicTask = PublicSettingsClient.PublicData;
            var privateTask = SettingsClient.PrivateData;

            var publicData = await publicTask;
            _eventbriteEnabled = publicData?.Events?.Eventbrite?.Enabled ?? false;

            var privateData = await privateTask;
            if (privateData?.Events?.Venues is not null)
                _venues = privateData.Events.Venues.ToList();

            _isLoading = false;
        }

        private static Timestamp? ToUtcTimestamp(DateOnly? date, int hour, int minute) =>
            date.HasValue
                ? Timestamp.FromDateTime(DateTime.SpecifyKind(date.Value.ToDateTime(new TimeOnly(hour, minute)), DateTimeKind.Utc))
                : null;

        private static string VenueDisplayName(GenericEventVenueRecord venue) =>
            venue.VenueOneOfCase == GenericEventVenueRecord.VenueOneOfOneofCase.Virtual
                ? (string.IsNullOrEmpty(venue.Virtual?.Name) ? "(Unnamed Virtual Venue)" : venue.Virtual.Name)
                : (string.IsNullOrEmpty(venue.Physical?.Name) ? "(Unnamed Venue)" : venue.Physical.Name);

        private void AddTag() => _tags.Add("");
        private void RemoveTag(int index)
        {
            if (index >= 0 && index < _tags.Count)
                _tags.RemoveAt(index);
        }

        private void OpenCreateTicketClassSheet()
        {
            _selectedTicketClass = null;
            _editClassName = "";
            _editClassType = GenericEventTicketClassType.TicketGeneralAccess;
            _editAmountAvailable = 0;
            _editPricePerTicketCents = 0;
            _editMaxTicketsPerUser = 0;
            _editIsTransferrable = false;
            _editCountTowardEventMax = true;
            _editRequiredSubscriptionAmountCents = 0;
            _editSaleStartDate = null;
            _editSaleStartHour = 0;
            _editSaleStartMinute = 0;
            _editSaleEndDate = null;
            _editSaleEndHour = 0;
            _editSaleEndMinute = 0;
            _isSheetOpen = true;
        }

        private void OpenEditTicketClassSheet(GenericTicketClassRecord ticketClass)
        {
            _selectedTicketClass = ticketClass;
            _editClassName = ticketClass.Name;
            _editClassType = ticketClass.Type;
            _editAmountAvailable = ticketClass.AmountAvailable;
            _editPricePerTicketCents = ticketClass.PricePerTicketCents;
            _editMaxTicketsPerUser = ticketClass.MaxTicketsPerUser;
            _editIsTransferrable = ticketClass.IsTransferrable;
            _editCountTowardEventMax = ticketClass.CountTowardEventMax;
            _editRequiredSubscriptionAmountCents = ticketClass.RequiredSubscriptionAmountCents;

            LoadSaleWindow(ticketClass.SaleStartOnUTC, ref _editSaleStartDate, ref _editSaleStartHour, ref _editSaleStartMinute);
            LoadSaleWindow(ticketClass.SaleEndOnUTC, ref _editSaleEndDate, ref _editSaleEndHour, ref _editSaleEndMinute);

            _isSheetOpen = true;
        }

        private static void LoadSaleWindow(Timestamp? ts, ref DateOnly? date, ref int hour, ref int minute)
        {
            if (ts is null)
            {
                date = null;
                hour = 0;
                minute = 0;
                return;
            }

            var dt = ts.ToDateTime();
            date = DateOnly.FromDateTime(dt);
            hour = dt.Hour;
            minute = dt.Minute;
        }

        private void CloseTicketClassSheet()
        {
            _selectedTicketClass = null;
            _isSheetOpen = false;
        }

        private void SaveTicketClassToList()
        {
            var ticketClass = _selectedTicketClass ?? new GenericTicketClassRecord { TicketClassID = Guid.NewGuid().ToString() };

            ticketClass.Name = _editClassName;
            ticketClass.Type = _editClassType;
            ticketClass.AmountAvailable = _editAmountAvailable;
            ticketClass.PricePerTicketCents = _editPricePerTicketCents;
            ticketClass.MaxTicketsPerUser = _editMaxTicketsPerUser;
            ticketClass.IsTransferrable = _editIsTransferrable;
            ticketClass.CountTowardEventMax = _editCountTowardEventMax;

            // Only meaningful alongside TICKET_MEMBER_LEVEL_ACCESS; clear it otherwise
            // so a stale value cannot ride along after the type is changed.
            ticketClass.RequiredSubscriptionAmountCents =
                _editClassType == GenericEventTicketClassType.TicketMemberLevelAccess
                    ? _editRequiredSubscriptionAmountCents
                    : 0;

            // Leave the Timestamps unset when no date was picked — proto-JSON
            // rejects empty strings for google.protobuf.Timestamp.
            ticketClass.SaleStartOnUTC = ToUtcTimestamp(_editSaleStartDate, _editSaleStartHour, _editSaleStartMinute);
            ticketClass.SaleEndOnUTC = ToUtcTimestamp(_editSaleEndDate, _editSaleEndHour, _editSaleEndMinute);

            if (_selectedTicketClass is null)
                _ticketClasses.Add(ticketClass);

            CloseTicketClassSheet();
        }

        private void RemoveTicketClass(GenericTicketClassRecord ticketClass) => _ticketClasses.Remove(ticketClass);

        private async Task SubmitCreate()
        {
            IsSaving = true;
            OverallError = null;

            try
            {
                var data = new EventData
                {
                    Title = _title,
                    Description = _description,
                    Location = _location,
                    VenueID = _venueId,
                    MaxTickets = _maxTickets,
                    InternalNotes = _internalNotes,
                    // Setting this while Eventbrite is disabled fails the whole create.
                    SyncToEventbrite = _syncToEventbrite && _eventbriteEnabled,
                };

                data.Tags.AddRange(_tags.Where(t => !string.IsNullOrWhiteSpace(t)));
                data.TicketClasses.AddRange(_ticketClasses);

                data.StartOnUTC = ToUtcTimestamp(_startDate, _startHour, _startMinute);
                data.EndOnUTC = ToUtcTimestamp(_endDate, _endHour, _endMinute);

                var req = new AdminCreateEventRequest { Data = data };
                var res = await AdminEventClient.AdminCreateEventAsync(req, UserHelper.GetGrpcCallOptions());

                if (res?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } err)
                {
                    OverallError = !string.IsNullOrEmpty(err.Message) ? err.Message : err.Reason.ToString();
                    ToastService.Error(OverallError);
                    return;
                }

                ToastService.Success("Event created successfully.");
                NavigationManager.NavigateTo("/events");
            }
            catch (Exception ex)
            {
                OverallError = ex.Message;
                ToastService.Error(OverallError);
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
