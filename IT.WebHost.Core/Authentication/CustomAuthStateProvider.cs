using System.Security.Claims;
using System.Text;
using System.Text.Json;
using IT.WebHost.Core.Clients;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace IT.WebHost.Core.Authentication
{
    public class CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
    {
        private AuthenticationState _currentState = new(new ClaimsPrincipal());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var ctx = httpContextAccessor.HttpContext;
            if (ctx is null)
                return Task.FromResult(_currentState);

            var token = ctx.Request.Cookies[AuthClient.CookieName];
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

            var user = ParseJwt(token);
            _currentState = new AuthenticationState(user);
            return Task.FromResult(_currentState);
        }

        public void NotifyUserChanged(ClaimsPrincipal user)
        {
            _currentState = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(_currentState));
        }

        private static ClaimsPrincipal ParseJwt(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return new ClaimsPrincipal();

            var payload = parts[1];
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            payload = payload.Replace('-', '+').Replace('_', '/');

            string json;
            try { json = Encoding.UTF8.GetString(Convert.FromBase64String(payload)); }
            catch { return new ClaimsPrincipal(); }

            var claims = new List<Claim>();
            using var doc = JsonDocument.Parse(json);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        claims.Add(new Claim(prop.Name, prop.Value.GetString()!));
                        break;
                    case JsonValueKind.Number:
                        claims.Add(new Claim(prop.Name, prop.Value.ToString()));
                        break;
                    case JsonValueKind.Array:
                        foreach (var item in prop.Value.EnumerateArray())
                            if (item.ValueKind == JsonValueKind.String)
                                claims.Add(new Claim(prop.Name, item.GetString()!));
                        break;
                }
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        }
    }
}
