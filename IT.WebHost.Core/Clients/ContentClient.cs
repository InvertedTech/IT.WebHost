using System.Reflection;
using System.Text;
using Google.Protobuf;
using IT.WebServices.Fragments.Content;

namespace IT.WebHost.Core.Clients
{
    public class ContentClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ContentClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public async Task<GetAllContentResponse> GetAllContentAsync(GetAllContentRequest request)
        {
            var qs = BuildQueryString(request);
            var url = string.IsNullOrEmpty(qs)
                ? $"{_baseUrl}/cms/content"
                : $"{_baseUrl}/cms/content?{qs}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<GetAllContentResponse>(json);
        }

        public async Task<GetContentResponse> GetContentByIdAsync(string contentId)
        {
            var url = $"{_baseUrl}/cms/content/{Uri.EscapeDataString(contentId)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonParser.Default.Parse<GetContentResponse>(json);
        }

        private static string BuildQueryString(object obj)
        {
            var sb = new StringBuilder();
            foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;

                var type = prop.PropertyType;
                if (!type.IsPrimitive && type != typeof(string) && !type.IsEnum) continue;

                var value = prop.GetValue(obj);
                if (value == null) continue;

                var defaultVal = type.IsValueType ? Activator.CreateInstance(type) : null;
                if (value.Equals(defaultVal)) continue;

                if (sb.Length > 0) sb.Append('&');
                sb.Append(Uri.EscapeDataString(prop.Name));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(value.ToString()!));
            }
            return sb.ToString();
        }
    }
}
