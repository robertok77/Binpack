using System.Net.Http.Headers;
using Binpack.Infrastructure.Abstractions;
using Binpack.Infrastructure.Services;
using Binpack.Web.Abstractions;
using Binpack.Web.ViewModel;
using Microsoft.Extensions.Options;

namespace Binpack.Web;

public static partial class Program
{
    public static WebApplicationBuilder ConfigurePort(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<PortServiceSettings>(builder.Configuration.GetSection(PortServiceSettings.Section));
        builder.Services.AddHttpClient<PortService>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptionsMonitor<PortServiceSettings>>().CurrentValue;
            client.BaseAddress = new Uri(opts.BaseAddress);
            client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });
        builder.Services.AddTransient<IPortService>(sp => sp.GetRequiredService<PortService>()); 
        builder.Services.AddTransient<IPortVM,PortVM>();
        return builder;
    }
}