using Microsoft.Extensions.DependencyInjection;
using RingCentral.Subscription.DI;
using RingCentral.Subscription.Helpers;
using RingCentral.Subscription.Services;

namespace RingCentral.Subscription;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            ConsolePrinter.PrintBanner();
            EnvLoader.Initialize(args);

            var host = HostBuilderFactory.CreateHost();
            await host.StartAsync();

            var menuService = host.Services.GetRequiredService<MenuService>();
            await menuService.RunAsync();
        }
        catch (Exception ex)
        {
            ConsolePrinter.Error($"An unhandled exception occurred: {ex.Message}");
            ConsolePrinter.Error(ex.ToString());

            Console.ReadKey();
        }
    }
}
