using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin;

public partial class Content
{
    [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    private IEnumerable<ContentListRecord> content = new List<ContentListRecord>();

    private Task LoadContentAsync() => throw new NotImplementedException();
}
