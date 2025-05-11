using Microsoft.Extensions.Hosting;

namespace RingCentral.Subscription.Services;

/// <summary>
/// Hosted service that ensures the <see cref="RcClientProvider"/> is initialized during application startup.
/// </summary>
public class RcInitializer(RcClientProvider rcClientProvider) : IHostedService
{
    /// <summary>
    /// Initializes the RingCentral client provider when the application starts.
    /// This includes setting up credentials and performing authorization.
    /// </summary>
    /// <param name="cancellationToken">Token that signals cancellation of the startup process.</param>
    /// <returns>A task that completes when initialization is finished.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the RingCentral client could not be initialized or authorized properly.
    /// </exception>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await rcClientProvider.InitializeAsync();

        if (rcClientProvider.Client == null)
            throw new InvalidOperationException("RingCentral client failed to initialize.");
    }

    /// <summary>
    /// Stops the hosted service. No shutdown tasks are required for this service.
    /// </summary>
    /// <param name="cancellationToken">Token that signals cancellation of the shutdown process.</param>
    /// <returns>A completed task.</returns>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
