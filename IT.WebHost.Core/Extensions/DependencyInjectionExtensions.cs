using IT.WebHost.Core.Authentication;
using IT.WebHost.Core.Clients;
using IT.WebHost.Core.Services;
using IT.WebServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAuthenticationClasses(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = JwtExtensions.GetPublicKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies[JwtExtensions.JWT_COOKIE_NAME];

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();

            services.AddCascadingAuthenticationState();

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
            services.AddScoped<UserTokenService>();
            return services;
        }
    }
}
