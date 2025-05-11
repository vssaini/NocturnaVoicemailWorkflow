using Microsoft.Extensions.Hosting;
using Nocturna.Application.Constants;
using Nocturna.Presentation.Extensions;
using Nocturna.Presentation.Helpers;
using Nocturna.Presentation.Middlewares;
using Serilog;

var host = new HostBuilder()
    .AddAppConfiguration()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
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