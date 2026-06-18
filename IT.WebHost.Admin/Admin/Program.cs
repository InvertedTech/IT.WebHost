using IT.Web.Project1.Services;
using IT.WebServices.Authentication;
using NeoUI.Blazor;
using NeoUI.Blazor.Extensions;
using NeoUI.Blazor.Primitives.Extensions;

namespace Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("RequireAnyAdminRole", policy =>
                {
                    policy.RequireRole(
                            RoleAbilities.ROLE_OWNER,
                            RoleAbilities.ROLE_ADMIN,
                            RoleAbilities.ROLE_BACKUP,
                            RoleAbilities.ROLE_OPS,
                            RoleAbilities.ROLE_SERVICE,
                            RoleAbilities.ROLE_CONTENT_PUBLISHER,
                            RoleAbilities.ROLE_CONTENT_WRITER,
                            RoleAbilities.ROLE_COMMENT_MODERATOR,
                            RoleAbilities.ROLE_COMMENT_APPELLATE_JUDGE,
                            RoleAbilities.ROLE_BOT_VERIFICATION,
                            RoleAbilities.ROLE_EVENT_MANAGER,
                            RoleAbilities.ROLE_EVENT_TICKET_MANAGER,
                            RoleAbilities.ROLE_MEMBER_MANAGER,
                            RoleAbilities.ROLE_SUBSCRIPTION_MANAGER
                        );
                });
            });

            builder.Services.AddNeoUIPrimitives();
            builder.Services.AddNeoUIComponents();
            builder.Services.AddGrpcClientClasses();
            builder.Services.AddCoreServices();
            builder.Services.AddAuthenticationClasses();
            builder.Services.AddSingleton<IToastService, ToastService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseAntiforgery();
            app.MapStaticAssets();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapGet("/auth/set-cookie", (string token, string? returnUrl, HttpContext ctx) =>
            {
                ctx.Response.Cookies.Append(JwtExtensions.JWT_COOKIE_NAME, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });
                return Results.Redirect(returnUrl ?? "/");
            });

            app.MapGet("/auth/logout", (HttpContext ctx) =>
            {
                ctx.Response.Cookies.Delete(JwtExtensions.JWT_COOKIE_NAME);
                return Results.Redirect("/");
            });

            app.Run();
        }
    }
}
