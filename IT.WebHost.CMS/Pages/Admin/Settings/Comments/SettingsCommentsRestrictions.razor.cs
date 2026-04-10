using IT.WebServices.Clients.Settings;
using IT.WebServices.Fragments.Comment;
using IT.WebServices.Fragments.Settings;
using Microsoft.AspNetCore.Components;

namespace IT.WebHost.CMS.Pages.Admin.Settings.Comments;

public partial class SettingsCommentsRestrictions
{
    [Inject] private SettingsClient SettingsClient { get; set; } = null!;

    private CommentRestrictionMinimumEnum _minimum = CommentRestrictionMinimumEnum.Anonymous;
    private float _level;

    protected override async Task OnInitializedAsync()
    {
        var data = await SettingsClient.PublicData;
        var r = data.Comments?.DefaultRestriction;
        if (r is null) return;

        _minimum = r.Minimum;
        _level = r.Level;
    }

    private async Task HandleSubmit()
    {
        var data = await SettingsClient.PublicData;
        var record = (data.Comments ?? new CommentsPublicRecord()).Clone();
        record.DefaultRestriction ??= new CommentRestrictionMinimum();
        record.DefaultRestriction.Minimum = _minimum;
        record.DefaultRestriction.Level = _level;

        await SettingsClient.ModifyCommentsPublicSettings(new ModifyCommentsPublicDataRequest { Data = record });
    }
}
