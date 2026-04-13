using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Comments;

public partial class SettingsCommentsModeration
{
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _isEditing = false;
    private List<string> _blackList = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var data = await SettingsClient.PrivateData;
        _blackList = data.Comments?.BlackList?.ToList() ?? new();
    }

    private void StartEdit() => _isEditing = true;

    private async Task CancelEdit()
    {
        _isEditing = false;
        await LoadAsync();
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
        _isEditing = false;
    }
}
