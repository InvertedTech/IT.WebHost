using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace IT.WebHost.CMS.Auth;

/// <summary>
/// Temporary stub — replace with real authentication provider.
/// </summary>
public class StubAuthenticationStateProvider : AuthenticationStateProvider
{
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "Admin"),
        ], authenticationType: "stub");

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }
}
