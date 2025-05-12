using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RingCentral.Subscription.Models;
using RingCentral.Subscription.Services;

namespace RingCentral.Subscription.DI;

/// <summary>
/// Provides a method to create and configure the application host for the RingCentral subscription console app.
/// </summary>
public static class HostBuilderFactory
{
    /// <summary>
    /// Creates and configures the application <see cref="IHost"/> instance.
    /// </summary>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    /// <item>Loads environment variables from a <c>.env</c> file using <c>dotenv.net</c>.</item>
    /// <item>Adds <c>appsettings.json</c> as a configuration source.</item>
    /// <item>Registers <see cref="RcClientProvider"/>, <see cref="RcSubscriptionManager"/>, <see cref="MenuService"/>, and <see cref="RcInitializer"/> in the DI container.</item>
    /// </list>
    /// </remarks>
    /// <returns>A fully configured <see cref="IHost"/> instance.</returns>
    public static IHost CreateHost()
    {
        // Ref - https://github.com/bolorundurowb/dotenv.net
        // Load environment variables from .env file
        DotEnv.Load();

        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.console.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<SubscriptionSetting>(context.Configuration.GetSection("RingCentral:Subscription"));

                services.AddSingleton<RcClientProvider>();
                services.AddSingleton<MenuService>();
                services.AddSingleton<RcSubscriptionManager>();
                services.AddHostedService<RcInitializer>();
            })
            .Build();
    }
}
