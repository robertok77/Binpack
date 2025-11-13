using Binpack.Web.Components;

namespace Binpack.Web;
public static partial class Program
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigurePort();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        return builder;
    }

    public static WebApplication Configure(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }

}
