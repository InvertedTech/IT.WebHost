using IT.WebHost.Core.Authentication;
using IT.WebHost.Core.Clients;
using IT.WebHost.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAuthenticationClasses(this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            services.AddHttpContextAccessor();
            return services;
        }

        public static IServiceCollection AddCoreClients(this IServiceCollection services)
        {
            services.AddHttpClient("content");
            services.AddSingleton<ContentClient>(sp =>
            {
                var baseUrl = sp.GetRequiredService<IConfiguration>()["ApiUrl"] ?? "http://localhost:8001/api";
                return new ContentClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("content"), baseUrl);
            });

            services.AddHttpClient("auth");
            services.AddSingleton<AuthClient>(sp =>
            {
                var baseUrl = sp.GetRequiredService<IConfiguration>()["ApiUrl"] ?? "http://localhost:8001/api";
                return new AuthClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("auth"), baseUrl);
            });

            services.AddHttpClient("settings");
            services.AddSingleton<SettingsClient>(sp =>
            {
                var baseUrl = sp.GetRequiredService<IConfiguration>()["ApiUrl"] ?? "http://localhost:8001/api";
                return new SettingsClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("settings"), baseUrl);
            });

            return services;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<TemplateService>();
            services.AddSingleton<SiteSettingsService>();
            return services;
        }
    }
}
