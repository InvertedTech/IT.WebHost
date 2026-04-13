using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Comment;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Comments;

public partial class SettingsCommentsRestrictions
{
    [Inject] private PublicSettingsClient PublicSettingsClient { get; set; } = null!;
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _isEditing = false;
    private CommentRestrictionMinimumEnum _minimum = CommentRestrictionMinimumEnum.Anonymous;
    private float _level;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var data = await PublicSettingsClient.PublicData;
        var r = data.Comments?.DefaultRestriction;
        if (r is null) return;

        _minimum = r.Minimum;
        _level = r.Level;
    }

    private void StartEdit() => _isEditing = true;

    private async Task CancelEdit()
    {
        _isEditing = false;
        await LoadAsync();
    }

    private async Task HandleSubmit()
    {
        var data = await PublicSettingsClient.PublicData;
        var record = (data.Comments ?? new CommentsPublicRecord()).Clone();
        record.DefaultRestriction ??= new CommentRestrictionMinimum();
        record.DefaultRestriction.Minimum = _minimum;
        record.DefaultRestriction.Level = _level;

        await SettingsClient.ModifyCommentsPublicSettings(new ModifyCommentsPublicDataRequest { Data = record });
        _isEditing = false;
    }
}
