using Google.Protobuf.Reflection;
using IT.WebHost.Api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace IT.WebHost.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<ServicesEndpointSettings >(
                builder.Configuration.GetSection("Services")
            );
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}