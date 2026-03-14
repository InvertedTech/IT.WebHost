using System.Text;
using Google.Protobuf;
using IT.WebServices.Fragments.Authentication;

namespace IT.WebHost.Core.Clients
{
    public class AuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public const string CookieName = "token";

        public AuthClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<AuthenticateUserResponse> LoginAsync(AuthenticateUserRequest request)
        {
            var json = JsonFormatter.Default.Format(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/login", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<AuthenticateUserResponse>(responseJson);
        }

        public async Task<CreateUserResponse> SignUpAsync(CreateUserRequest request)
        {
            var json = JsonFormatter.Default.Format(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/auth/createuser", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<CreateUserResponse>(responseJson);
        }
    }
}
