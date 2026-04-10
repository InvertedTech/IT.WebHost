using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Comments;

public partial class SettingsCommentsModeration
{
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private List<string> _blackList = new();

    protected override async Task OnInitializedAsync()
    {
        var data = await SettingsClient.PrivateData;
        _blackList = data.Comments?.BlackList?.ToList() ?? new();
    }

    private void AddEntry() => _blackList.Add(string.Empty);

    private void RemoveEntry(int index) => _blackList.RemoveAt(index);

    private async Task HandleSubmit()
    {
        var data = await SettingsClient.PrivateData;
        var record = (data.Comments ?? new CommentsPrivateRecord()).Clone();
        record.BlackList.Clear();
        record.BlackList.AddRange(_blackList);

        await SettingsClient.ModifyCommentsPrivateSettings(new ModifyCommentsPrivateDataRequest { Data = record });
    }
}
