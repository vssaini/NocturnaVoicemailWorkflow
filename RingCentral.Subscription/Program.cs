using Microsoft.Extensions.DependencyInjection;
using RingCentral.Subscription.DI;
using RingCentral.Subscription.Helpers;
using RingCentral.Subscription.Services;

namespace RingCentral.Subscription;

class Program
{
    static async Task Main()
    {
        ConsolePrinter.PrintBanner();

        var host = HostBuilderFactory.CreateHost();
        await host.StartAsync();

        var menuService = host.Services.GetRequiredService<MenuService>();
        await menuService.RunAsync();
    }
}