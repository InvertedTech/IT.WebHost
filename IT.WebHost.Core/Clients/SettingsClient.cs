using Google.Protobuf;
using IT.WebServices.Fragments.Settings;

namespace IT.WebHost.Core.Clients
{
    public class SettingsClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SettingsClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<SettingsPublicData> GetPublicDataAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/settings/public");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<GetPublicDataResponse>(json).Public;
        }
    }
}
