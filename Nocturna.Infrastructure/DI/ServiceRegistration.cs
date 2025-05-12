using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Abstractions;
using Nocturna.Infrastructure.Persistence;
using Nocturna.Infrastructure.Persistence.Repositories;
using Nocturna.Infrastructure.RingCentral;
using Nocturna.Infrastructure.RingCentral.Clients;
using Nocturna.Infrastructure.RingCentral.Handlers;
using Nocturna.Infrastructure.Services;
using Refit;
using System.Text.Json;
using Nocturna.Domain.Config;

namespace Nocturna.Infrastructure.DI;

/// <summary>
/// Provides extension methods for registering infrastructure services in the dependency injection container.
/// </summary>
public static class ServiceRegistration
{
    /// <summary>
    /// Registers all infrastructure services, including RingCentral clients, database connections, and repositories.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<RingCentralJwtAuthHandler>();

        services.AddRingCentralSettings();
        services.AddRingCentralMediaRefitClient();
        services.AddDependencies();

        return services;
    }

    /// <summary>
    /// Configures and validates RingCentral settings from the application configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add options to.</param>
    private static void AddRingCentralSettings(this IServiceCollection services)
    {
        services.AddOptions<RingCentralSettings>()
            .BindConfiguration("RingCentral")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<FtpSettings>()
            .BindConfiguration("Ftp")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    /// <summary>
    /// Creates Refit settings with JSON serialization configured for PascalCase property names.
    /// </summary>
    /// <returns>A <see cref="RefitSettings"/> instance with configured JSON serialization.</returns>
    private static RefitSettings CreateRefitSettings()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null // Ensures PascalCase serialization
        };

        return new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions)
        };
    }

    /// <summary>
    /// Registers the RingCentral media API client with Refit and configures JWT authentication.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    private static void AddRingCentralMediaRefitClient(this IServiceCollection services)
    {
        var refitSettings = CreateRefitSettings();

        services.AddRefitClient<IRingCentralMediaApi>(refitSettings)
            .ConfigureHttpClient((provider, client) =>
            {
                var rcSettings = provider.GetRequiredService<IOptions<RingCentralSettings>>().Value;
                client.BaseAddress = new Uri(rcSettings.MediaUrl);
            })
            .AddHttpMessageHandler<RingCentralJwtAuthHandler>();
    }

    /// <summary>
    /// Registers additional dependencies such as database connection factories, repositories, and services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add dependencies to.</param>
    private static void AddDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IVoicemailRepository, VoicemailRepository>();

        services.AddScoped<IValidationTokenService, ValidationTokenService>();
        services.AddScoped<ISecurityService, SecurityService>();

        services.AddScoped<IVoicemailWebhookParser, VoicemailWebhookParser>();
        services.AddScoped<IVoicemailProcessor, VoicemailProcessor>();
        services.AddScoped<ITranscriptFetcher, TranscriptFetcher>();

        services.AddTransient<IFtpClientService, FtpClientService>();
        services.AddTransient<IExcelFileService, ExcelFileService>();
        services.AddScoped<IFtpFileService, FtpFileService>();

        services.AddScoped<ITranscriptionWriter, TranscriptionWriter>();
    }
}