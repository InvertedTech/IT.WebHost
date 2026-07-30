using Admin.Models;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Careers;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Careers
{
    public partial class ViewCareer
    {
        [Parameter]
        public string CareerId { get; set; } = null!;

        [Inject]
        public ONUserHelper UserHelper { get; set; } = null!;
        [Inject]
        public CareersInterface.CareersInterfaceClient CareerClient { get; set; } = null!;
        [Inject]
        private IToastService ToastService { get; set; } = null!;

        private CareerRecord? Career { get; set; }
        private CareerEditModel? EditCareer { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsEditing { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            IsLoading = true;
            IsEditing = false;
            Career = null;
            await LoadCareer();
            EditCareer = Career is not null ? CareerEditModel.FromRecord(Career) : null;
            IsLoading = false;
        }

        private async Task LoadCareer()
        {
            var res = await CareerClient.GetCareerAsync(
                new GetCareerRequest { CareerId = CareerId },
                UserHelper.GetGrpcCallOptions());

            Career = res?.Career;
        }

        private void StartEdit() => IsEditing = true;

        private async Task CancelEdit()
        {
            IsEditing = false;
            await LoadCareer();
            EditCareer = Career is not null ? CareerEditModel.FromRecord(Career) : null;
        }

        private async Task SaveCareer()
        {
            if (EditCareer is null)
            {
                return;
            }

            var res = await CareerClient.UpdateCareerAsync(
                new UpdateCareerRequest
                {
                    CareerId = CareerId,
                    Career = EditCareer.ToRecord(),
                },
                UserHelper.GetGrpcCallOptions());

            if (res?.Error is null || res.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                ToastService.Success("Career updated successfully.");
                IsEditing = false;
                await LoadCareer();
                EditCareer = Career is not null ? CareerEditModel.FromRecord(Career) : null;
            }
            else
            {
                var message = !string.IsNullOrEmpty(res.Error.Message)
                    ? res.Error.Message
                    : res.Error.Reason.ToString();

                ToastService.Error(message);
            }
        }
    }
}
