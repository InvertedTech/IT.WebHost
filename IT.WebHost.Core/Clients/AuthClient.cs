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

        public async Task<GetOwnUserResponse> GetOwnUserAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/auth/user");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<GetOwnUserResponse>(responseJson);
        }

        public async Task<ModifyOwnUserResponse> ModifyOwnUserAsync(string token, ModifyOwnUserRequest request)
        {
            var json = JsonFormatter.Default.Format(request);
            var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/auth/user");
            req.Headers.Authorization = GetAuthHeader(token);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(req);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<ModifyOwnUserResponse>(responseJson);
        }

        public async Task<ChangeOwnPasswordResponse> ChangeOwnPasswordAsync(string token, ChangeOwnPasswordRequest request)
        {
            var json = JsonFormatter.Default.Format(request);
            var req = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/auth/password");
            req.Headers.Authorization = GetAuthHeader(token);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(req);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<ChangeOwnPasswordResponse>(responseJson);
        }

        private System.Net.Http.Headers.AuthenticationHeaderValue GetAuthHeader(string? token)
        {
            return new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
