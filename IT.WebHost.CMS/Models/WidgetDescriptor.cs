namespace IT.WebHost.CMS.Models
{
    public record WidgetDescriptor(
        string Id,
        string DisplayName,
        string Icon,
        Type ComponentType
    );
}
