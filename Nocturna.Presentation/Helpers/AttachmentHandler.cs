using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Functions.Activities;

namespace Nocturna.Presentation.Helpers;

/// <summary>
/// Provides logic to fetch an attachment Id with retry capabilities using Durable Functions.
/// </summary>
public static class AttachmentHandler
{
    /// <summary>
    /// Attempts to fetch an attachment Id, retrying with predefined delays if the initial fetch fails.
    /// </summary>
    /// <param name="orchestrationContext">The orchestration context from Durable Functions.</param>
    /// <param name="activityContext">The activity context with payload containing necessary data to attempt fetching the attachment.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the attachment Id if found;
    /// otherwise, <c>null</c> after all retries fail.
    /// </returns>
    public static async Task<long?> FetchAttachmentIdAsync(
        TaskOrchestrationContext orchestrationContext,
        ActivityContext<WebhookPayloadDto> activityContext)
    {
        var initialAttachmentId = await orchestrationContext.CallActivityAsync<long?>(nameof(FetchTransAttachId), activityContext);
        if (initialAttachmentId.HasValue)
            return initialAttachmentId;

        var logger = orchestrationContext.CreateReplaySafeLogger(typeof(AttachmentHandler));
        logger.LogWarning("Payload {PayloadUuid} - Attachment Id not available in payload. Starting retry sequence...", activityContext.PayloadUuid);
        
        var retryDelays = new[]
        {
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(40),
            TimeSpan.FromSeconds(80),
        };

        return await RetryAsync(orchestrationContext, activityContext, retryDelays, logger);
    }

    /// <summary>
    /// Executes a series of retry attempts with increasing delays to fetch the attachment Id.
    /// </summary>
    /// <param name="context">The orchestration context.</param>
    /// <param name="activityContext">The activity context with webhook payload DTO.</param>
    /// <param name="retryDelays">An array of delays between retry attempts.</param>
    /// <param name="logger">Logger for logging retry events.</param>
    /// <returns>
    /// A task representing the retry operation. The task result contains the attachment Id if successfully retrieved,
    /// or <c>null</c> if all attempts fail.
    /// </returns>
    private static async Task<long?> RetryAsync(
        TaskOrchestrationContext context,
        ActivityContext<WebhookPayloadDto> activityContext,
        TimeSpan[] retryDelays,
        ILogger logger)
    {
        for (int i = 0; i < retryDelays.Length; i++)
        {
            var attempt = i + 1;
            var delay = retryDelays[i];

            var result = await HandleRetryAttemptAsync(context, activityContext, logger, attempt, delay);
            if (result.HasValue)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Performs a single retry attempt after a delay and logs the result.
    /// </summary>
    /// <param name="orchestrationContext">The orchestration context.</param>
    /// <param name="activityContext">The activity context with payload containing the retry data.</param>
    /// <param name="logger">Logger to record success or failure.</param>
    /// <param name="attempt">The current attempt number.</param>
    /// <param name="delay">The delay before making the attempt.</param>
    /// <returns>
    /// A task that returns the attachment Id if found on this attempt; otherwise, <c>null</c>.
    /// </returns>
    private static async Task<long?> HandleRetryAttemptAsync(
        TaskOrchestrationContext orchestrationContext,
        ActivityContext<WebhookPayloadDto> activityContext,
        ILogger logger,
        int attempt,
        TimeSpan delay)
    {
        delay = AddJitter(delay);
        LogRetryWaiting(logger, attempt, delay, activityContext.PayloadUuid);

        var fireAt = orchestrationContext.CurrentUtcDateTime.Add(delay);
        await orchestrationContext.CreateTimer(fireAt, CancellationToken.None);

        var attachmentId = await orchestrationContext.CallActivityAsync<long?>(nameof(FetchTransAttachIdFromRc), activityContext);

        if (attachmentId.HasValue)
        {
            LogRetrySuccess(logger, activityContext.PayloadUuid);
            return attachmentId;
        }

        LogRetryFailure(logger, attempt, activityContext.PayloadUuid);
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
        logger.LogInformation("Payload {PayloadUuid} - [FetchAttachId: RETRY {Attempt}, WAIT] 🕒 Waiting {DelaySeconds}s before next attempt to fetch attachment",
            payloadUuid, attempt, delay.TotalSeconds);


    /// <summary>
    /// Logs a successful retry attempt.
    /// </summary>
    private static void LogRetrySuccess(ILogger logger, string payloadUuid) =>
        logger.LogInformation("Payload {PayloadUuid} - Successfully fetched attachment Id", payloadUuid);

    /// <summary>
    /// Logs a failed retry attempt.
    /// </summary>
    private static void LogRetryFailure(ILogger logger, int attempt, string payloadUuid) =>
        logger.LogWarning("Payload {PayloadUuid} - [FetchAttachId: RETRY {Attempt}, FAIL] Failed to fetch attachment",
            payloadUuid, attempt);

}

