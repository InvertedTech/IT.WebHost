using System.Reflection;
using BlazorBlueprint.Components;
using BlazorBlueprint.Primitives.Services;

namespace IT.WebHost.CMS.Pages;

public partial class Index
{
    private string? inputValue;
    private bool agreeTerms;
    private bool notifications = true;
    private bool dialogOpen;

    private static readonly string componentsVersion = typeof(BbButton).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "?";

    private static readonly string primitivesVersion = typeof(IPortalService).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "?";
}
