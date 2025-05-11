using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nocturna.Infrastructure.DI;

namespace Nocturna.Presentation.Extensions
{
    /// <summary>
    /// Extension methods for configuring the HostBuilder.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Adds application configuration sources to the HostBuilder.
        /// Loads appsettings.json, environment-specific appsettings, user secrets (in Development),
        /// and environment variables.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> to configure.</param>
        /// <returns>The same <see cref="IHostBuilder"/> instance for chaining.</returns>
        public static IHostBuilder AddAppConfiguration(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                var env = context.HostingEnvironment;

                configBuilder
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                    configBuilder.AddUserSecrets<Program>();

                configBuilder.AddEnvironmentVariables();
            });
        }

        /// <summary>
        /// Registers application services and infrastructure services with the dependency injection container.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> to configure.</param>
        /// <returns>The same <see cref="IHostBuilder"/> instance for chaining.</returns>
        public static IHostBuilder RegisterServices(this IHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                services.AddFunctionServices(context.Configuration);
                services.AddInfrastructureServices();
            });
        }
    }
}
