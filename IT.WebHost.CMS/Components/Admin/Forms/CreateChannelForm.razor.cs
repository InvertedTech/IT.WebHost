using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Components.Admin.Forms;

public partial class CreateChannelForm
{
    [Parameter] public IEnumerable<ChannelRecord> ParentChannels { get; set; } = [];
    [Parameter] public EventCallback<ChannelRecord> OnSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private string displayName = string.Empty;
    private string urlStub = string.Empty;
    private string parentChannelId = string.Empty;
    private string youtubeUrl = string.Empty;
    private string rumbleUrl = string.Empty;

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

        await OnSubmit.InvokeAsync(new ChannelRecord
        {
            ChannelId       = Guid.NewGuid().ToString(),
            DisplayName     = displayName,
            UrlStub         = urlStub,
            ParentChannelId = parentChannelId,
            YoutubeUrl      = youtubeUrl,
            RumbleUrl       = rumbleUrl,
        });

        displayName     = string.Empty;
        urlStub         = string.Empty;
        parentChannelId = string.Empty;
        youtubeUrl      = string.Empty;
        rumbleUrl       = string.Empty;
    }
}
