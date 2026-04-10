using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.CMS;

public partial class SetingsCategories
{
    [Inject] private SettingsInterface.SettingsInterfaceClient SettingsClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private Task LoadCategoriesAsync() => throw new NotImplementedException();
    private Task SaveCategoriesAsync() => throw new NotImplementedException();
}
