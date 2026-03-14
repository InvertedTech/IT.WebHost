using System.Text;
using Google.Protobuf;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Http;

namespace IT.WebHost.Core.Clients
{
    public class AuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public const string CookieName = "auth_token";

        public AuthClient(HttpClient httpClient, string baseUrl, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticateUserResponse> LoginAsync(AuthenticateUserRequest request)
        {
            var json = JsonFormatter.Default.Format(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/login", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonParser.Default.Parse<AuthenticateUserResponse>(responseJson);
            if (result.Ok && !string.IsNullOrEmpty(result.BearerToken))
                SetAuthCookie(result.BearerToken);
            return result;
        }

        public async Task<CreateUserResponse> SignUpAsync(CreateUserRequest request)
        {
            var json = JsonFormatter.Default.Format(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/createuser", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonParser.Default.Parse<CreateUserResponse>(responseJson);
            if (!string.IsNullOrEmpty(result.BearerToken))
                SetAuthCookie(result.BearerToken);
            return result;
        }

        private void SetAuthCookie(string token)
        {
            var ctx = _httpContextAccessor.HttpContext;
            if (ctx is null) return;
            ctx.Response.Cookies.Append(CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
        }
    }
}
