using IT.WebServices.Authentication;
using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.CMS;

public partial class SetingsCategories
{
    [Inject] private CategoryHelper CategoryHelper { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;
    private IEnumerable<CategoryRecord> categories = new List<CategoryRecord>();
    private bool _isEditing = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        var foundCategories = CategoryHelper.GetAll();
        if (foundCategories != null)
            categories = foundCategories;
    }

    private void StartEdit() => _isEditing = true;

    private async Task CancelEdit()
    {
        _isEditing = false;
        await LoadCategoriesAsync();
    }

    private void HandleSave() => _isEditing = false;

    private Task SaveCategoriesAsync() => throw new NotImplementedException();
}
