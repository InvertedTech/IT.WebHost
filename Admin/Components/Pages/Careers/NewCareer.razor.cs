using Admin.Models;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Careers;
using Microsoft.AspNetCore.Components;
using NeoUI.Blazor;

namespace Admin.Components.Pages.Careers
{
    public partial class NewCareer
    {
        private const string BodyMarkdownTemplate = """
            ## About


            ## Role Overview


            ## Responsibilities

            -

            ## Qualifications

            -
            """;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private CareersInterface.CareersInterfaceClient CareerClient { get; set; } = null!;
        [Inject] private ONUserHelper UserHelper { get; set; } = null!;
        [Inject] private IToastService ToastService { get; set; } = null!;

        private CareerEditModel NewCareerModel { get; set; } = new()
        {
            EmploymentType = JobType.FullTime,
            BodyMarkdown = BodyMarkdownTemplate,
        };

        private bool IsSaving { get; set; }
        private string? OverallError { get; set; }

        private async Task SubmitCreate()
        {
            IsSaving = true;
            OverallError = null;

            try
            {
                var record = NewCareerModel.ToRecord();
                var req = new CreateCareerRequest
                {
                    Title = record.Title,
                    Company = record.Company,
                    Location = record.Location,
                    Department = record.Department,
                    Contact = record.Contact,
                    BodyMarkdown = record.BodyMarkdown,
                };

                var res = await CareerClient.CreateCareerAsync(req, UserHelper.GetGrpcCallOptions());

                if (res?.Error is { Reason: not APIErrorReason.ErrorReasonNoError } err)
                {
                    OverallError = !string.IsNullOrEmpty(err.Message) ? err.Message : err.Reason.ToString();
                    ToastService.Error(OverallError);
                    return;
                }

                ToastService.Success("Career created successfully.");
                NavigationManager.NavigateTo($"/careers/{res!.CareerId}");
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
