using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Content;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages;

public partial class ViewContent
{
    [Inject] private ContentInterface.ContentInterfaceClient ContentClient { get; set; } = null!;
    [Inject] private ONUserHelper UserHelper { get; set; } = null!;

    [Parameter] public string Slug { get; set; } = string.Empty;

    private ContentPublicRecord? _record;

    protected override async Task OnParametersSetAsync()
    {
        var res = await ContentClient.GetContentByUrlAsync(new GetContentByUrlRequest
        {
            ContentUrl = "/" + Slug
        }, UserHelper.GetGrpcCallOptions());

        _record = res?.Record;
    }
}
