using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Comment;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Comments;

public partial class SettingsCommentsGeneral
{
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private bool _allowLinks;
    private bool _explicitModeEnabled;
    private CommentOrder _defaultOrder = CommentOrder.Liked;

    protected override async Task OnInitializedAsync()
    {
        var data = await SettingsClient.PublicData;
        var c = data.Comments;
        if (c is null) return;

        _allowLinks = c.AllowLinks;
        _explicitModeEnabled = c.ExplicitModeEnabled;
        _defaultOrder = c.DefaultOrder;
    }

    private async Task HandleSubmit()
    {
        var data = await SettingsClient.PublicData;
        var record = (data.Comments ?? new CommentsPublicRecord()).Clone();
        record.AllowLinks = _allowLinks;
        record.ExplicitModeEnabled = _explicitModeEnabled;
        record.DefaultOrder = _defaultOrder;

        await SettingsClient.ModifyCommentsPublicSettings(new ModifyCommentsPublicDataRequest { Data = record });
    }
}
