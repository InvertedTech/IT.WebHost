using System.Security.Claims;
using IT.WebHost.Core.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace IT.WebHost.Core.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal());

        private readonly AuthClient _authClient;
        private readonly string? _token;
        private readonly PersistentComponentState _persistentState;
        private readonly PersistingComponentStateSubscription _subscription;
        private AuthenticationState? _cachedState;

        public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor, AuthClient authClient, PersistentComponentState persistentState)
        {
            _authClient = authClient;
            _persistentState = persistentState;
            _token = httpContextAccessor.HttpContext?.Request.Cookies[AuthClient.CookieName];
            _subscription = _persistentState.RegisterOnPersisting(PersistAuthStateAsync);
        }

        private async Task PersistAuthStateAsync()
        {
            // Only persist during SSR prerender (when we have the HTTP token).
            // In the interactive circuit, _token is null and PersistAsJson would throw.
            if (string.IsNullOrEmpty(_token)) return;

            var state = await GetAuthenticationStateAsync();
            var user = state.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                var claims = user.Claims.Select(c => new ClaimData(c.Type, c.Value)).ToList();
                _persistentState.PersistAsJson("AuthClaims", claims);
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_cachedState is not null)
                return _cachedState;

            // Restore from persisted state when running in interactive circuit (no HttpContext)
            if (_persistentState.TryTakeFromJson<List<ClaimData>>("AuthClaims", out var persistedClaims) && persistedClaims is not null)
            {
                var claims = persistedClaims.Select(c => new Claim(c.Type, c.Value)).ToList();
                return _cachedState = new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, "api")));
            }

            if (string.IsNullOrEmpty(_token))
                return _cachedState = Anonymous;

            try
            {
                var res = await _authClient.GetOwnUserAsync(_token);
                var record = res?.Record?.Public;
                if (record is null)
                    return _cachedState = Anonymous;

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, record.UserID),
                    new(ClaimTypes.Name, record.Data.UserName),
                };

                foreach (var role in res!.Record.Private?.Roles ?? [])
                    claims.Add(new Claim(ClaimTypes.Role, role));

                return _cachedState = new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity(claims, "api")));
            }
            catch
            {
                return _cachedState = Anonymous;
            }
        }

        public void NotifyLoggedOut()
        {
            _cachedState = Anonymous;
            NotifyAuthenticationStateChanged(Task.FromResult(_cachedState));
        }

        public void Dispose() => _subscription.Dispose();

        private record ClaimData(string Type, string Value);
    }
}
