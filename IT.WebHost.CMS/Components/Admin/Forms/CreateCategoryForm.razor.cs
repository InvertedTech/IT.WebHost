using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Components.Admin.Forms;

public partial class CreateCategoryForm
{
    [Parameter] public IEnumerable<CategoryRecord> ParentCategories { get; set; } = [];
    [Parameter] public EventCallback<CategoryRecord> OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private string displayName = string.Empty;
    private string urlStub = string.Empty;
    private string parentCategoryId = string.Empty;

    private string? errorMessage;

    private async Task HandleSubmit()
    {
        errorMessage = null;

        displayName = displayName.Trim();
        urlStub = urlStub.Trim();

        if (string.IsNullOrEmpty(displayName) || string.IsNullOrEmpty(urlStub))
        {
            errorMessage = "Display Name and URL Stub are required.";
            return;
        }

        await OnSubmit.InvokeAsync(new CategoryRecord
        {
            CategoryId       = Guid.NewGuid().ToString(),
            DisplayName      = displayName,
            UrlStub          = urlStub,
            ParentCategoryId = parentCategoryId,
        });

        displayName      = string.Empty;
        urlStub          = string.Empty;
        parentCategoryId = string.Empty;
    }
}
