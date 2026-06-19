using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddNeoUIPrimitives();
            builder.Services.AddNeoUIComponents();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAntiforgery();
            app.MapStaticAssets();

            app.MapRazorComponents<WebApp.App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
