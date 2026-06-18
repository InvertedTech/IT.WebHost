using IT.WebServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProtoValidate;

namespace IT.Web.Project1.Services
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

            services.AddScoped<ONUserHelper>();

            services.AddAuthorization();

            services.AddCascadingAuthenticationState();

            return services;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            //services.AddSingleton<TemplateService>();
            services.AddSingleton<SiteSettingsService>();
            services.AddProtoValidate();
            //services.AddScoped<UserTokenService>();
            return services;
        }
    }
}
