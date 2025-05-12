using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Functions.Activities;

namespace Nocturna.Presentation.Helpers;

/// <summary>
/// Provides logic to fetch an attachment Id with retry capabilities using Durable Functions.
/// </summary>
public static class AttachmentRetryHandler
{
    /// <summary>
    /// Attempts to fetch an attachment Id, retrying with predefined delays if the initial fetch fails.
    /// </summary>
    /// <param name="context">The orchestration context from Durable Functions.</param>
    /// <param name="payloadDto">The payload containing necessary data to attempt fetching the attachment.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the attachment Id if found;
    /// otherwise, <c>null</c> after all retries fail.
    /// </returns>
    public static async Task<long?> FetchWithRetryAsync(
        TaskOrchestrationContext context,
        WebhookPayloadDto payloadDto)
    {
        var logger = context.CreateReplaySafeLogger(typeof(AttachmentRetryHandler));

        var initialAttachmentId = await context.CallActivityAsync<long?>(nameof(FetchTranAttachId), payloadDto);
        if (initialAttachmentId.HasValue)
            return initialAttachmentId;

        logger.LogWarning("Attachment Id not found for payload {PayloadUuid}. Starting retry sequence...", payloadDto.Uuid);

        var retryDelays = new[]
        {
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(35),
            TimeSpan.FromSeconds(40),
            TimeSpan.FromSeconds(60),
        };

        return await RetryAsync(context, payloadDto, retryDelays, logger);
    }

    /// <summary>
    /// Executes a series of retry attempts with increasing delays to fetch the attachment Id.
    /// </summary>
    /// <param name="context">The orchestration context.</param>
    /// <param name="payloadDto">The webhook payload DTO.</param>
    /// <param name="retryDelays">An array of delays between retry attempts.</param>
    /// <param name="logger">Logger for logging retry events.</param>
    /// <returns>
    /// A task representing the retry operation. The task result contains the attachment Id if successfully retrieved,
    /// or <c>null</c> if all attempts fail.
    /// </returns>
    private static async Task<long?> RetryAsync(
        TaskOrchestrationContext context,
        WebhookPayloadDto payloadDto,
        TimeSpan[] retryDelays,
        ILogger logger)
    {
        for (int i = 0; i < retryDelays.Length; i++)
        {
            var attempt = i + 1;
            var delay = retryDelays[i];

            var result = await HandleRetryAttemptAsync(context, payloadDto, logger, attempt, delay);
            if (result.HasValue)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Performs a single retry attempt after a delay and logs the result.
    /// </summary>
    /// <param name="context">The orchestration context.</param>
    /// <param name="payloadDto">The payload containing the retry data.</param>
    /// <param name="logger">Logger to record success or failure.</param>
    /// <param name="attempt">The current attempt number.</param>
    /// <param name="delay">The delay before making the attempt.</param>
    /// <returns>
    /// A task that returns the attachment Id if found on this attempt; otherwise, <c>null</c>.
    /// </returns>
    private static async Task<long?> HandleRetryAttemptAsync(
        TaskOrchestrationContext context,
        WebhookPayloadDto payloadDto,
        ILogger logger,
        int attempt,
        TimeSpan delay)
    {
        LogRetryWaiting(logger, attempt, delay, payloadDto.Uuid);

        var fireAt = context.CurrentUtcDateTime.Add(AddJitter(delay));
        await context.CreateTimer(fireAt, CancellationToken.None);

        var attachmentId = await context.CallActivityAsync<long?>(nameof(FetchTranAttachIdFromRingCentral), payloadDto);

        if (attachmentId.HasValue)
        {
            LogRetrySuccess(logger, attempt, payloadDto.Uuid);
            return attachmentId;
        }

        LogRetryFailure(logger, attempt, payloadDto.Uuid);
        return null;
    }

    /// <summary>
    /// Adds a small randomized jitter to the given delay to prevent retry stampedes and spread out requests.
    /// </summary>
    /// <param name="baseDelay">The base delay duration before applying jitter.</param>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing the base delay plus a random jitter between 1 and 3 seconds.
    /// </returns>
    private static TimeSpan AddJitter(TimeSpan baseDelay)
    {
        var jitter = TimeSpan.FromSeconds(Random.Shared.Next(1, 4)); // add 1–3s
        return baseDelay + jitter;
    }

    /// <summary>
    /// Logs that the retry attempt is about to wait for a delay.
    /// </summary>
    private static void LogRetryWaiting(ILogger logger, int attempt, TimeSpan delay, string payloadUuid) =>
        logger.LogInformation(
            "[RETRY {Attempt}] - Waiting {DelaySeconds}s to fetch attachment Id for payload {PayloadUuid}.",
            attempt, delay.TotalSeconds, payloadUuid);

    /// <summary>
    /// Logs a successful retry attempt.
    /// </summary>
    private static void LogRetrySuccess(ILogger logger, int attempt, string payloadUuid) =>
        logger.LogInformation(
            "[RETRY {Attempt}] - Successfully fetched attachment Id for payload {PayloadUuid}.",
            attempt, payloadUuid);

    /// <summary>
    /// Logs a failed retry attempt.
    /// </summary>
    private static void LogRetryFailure(ILogger logger, int attempt, string payloadUuid) =>
        logger.LogWarning(
            "[RETRY {Attempt}] - Attempt failed for payload {PayloadUuid}.",
            attempt, payloadUuid);
}

