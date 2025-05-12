using Microsoft.Extensions.Hosting;
using Nocturna.Application.Constants;
using Nocturna.Presentation.Extensions;
using Nocturna.Presentation.Helpers;
using Nocturna.Presentation.Middlewares;
using Serilog;

// Note: Discarded the use of ConfigureFunctionsWebApplication
// to get rid of System.Net.Sockets.SocketException (10013): An attempt was made to access a socket in a way forbidden by its access permissions.

var host = new HostBuilder()
    .AddAppConfiguration()
    .ConfigureFunctionsWorkerDefaults(w =>
    {
        w.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .RegisterServices()
    .UseSerilog()
    .Build();

try
{
    StartupLogger.LogStartup();
    host.Run();
}
catch (Exception ex)
{
    var logger = Log.ForContext("SourceContext", AppConstants.SourceContext);
    logger.Fatal(ex, "Failed to start {AppName}.", AppConstants.AppName);
}
finally
{
    Log.CloseAndFlush();
}