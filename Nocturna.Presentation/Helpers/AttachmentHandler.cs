using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Enums;
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
        logger.LogInformation("Payload {PayloadUuid} - Attachment Id not available in payload. Starting retry sequence...", activityContext.PayloadUuid);
        
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
        for (var i = 0; i < retryDelays.Length; i++)
        {
            var attempt = i + 1;
            var delay = retryDelays[i];

            var result = await HandleRetryAttemptAsync(context, activityContext, logger, attempt, delay);

            if (result.AttachmentId.HasValue)
                return result.AttachmentId;

            if (!result.ShouldRetry)
                break; // Stop retrying early
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
    private static async Task<RetryResult> HandleRetryAttemptAsync(
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

        var transResult = await orchestrationContext.CallActivityAsync<TranscriptionResult>(nameof(FetchTransAttachIdFromRc), activityContext);

        return EvaluateTranscriptionResult(transResult, logger, attempt, activityContext.PayloadUuid);
    }

    /// <summary>
    /// Evaluates the transcription result and determines whether a retry should be attempted.
    /// </summary>
    /// <param name="result">The result of the transcription fetch attempt, containing the attachment ID and transcription status.</param>
    /// <param name="logger">The logger used to record success, failure, or termination due to final transcription state.</param>
    /// <param name="attempt">The current retry attempt number.</param>
    /// <param name="payloadUuid">The unique identifier for the payload being processed, used in log messages.</param>
    /// <returns>
    /// A <see cref="RetryResult"/> indicating whether the operation was successful or if further retries should be attempted.
    /// </returns>
    private static RetryResult EvaluateTranscriptionResult(
        TranscriptionResult result,
        ILogger logger,
        int attempt,
        string payloadUuid)
    {
        if (result.AttachmentId.HasValue)
        {
            LogRetrySuccess(logger, payloadUuid);
            return new RetryResult(result.AttachmentId, false);
        }

        if (result.TranscriptionStatus != VmTranscriptionStatus.InProgress)
        {
            LogRetryTerminated(logger, payloadUuid, result.TranscriptionStatus);
            return new RetryResult(null, false);
        }

        LogRetryFailure(logger, attempt, payloadUuid);
        return new RetryResult(null, true);
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
    /// Logs a warning that retries will be skipped due to a terminal transcription status.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="payloadUuid">The UUID of the payload being processed.</param>
    /// <param name="status">The final transcription status received.</param> //
    private static void LogRetryTerminated(ILogger logger, string payloadUuid, VmTranscriptionStatus status) =>
        logger.LogWarning("Payload {PayloadUuid} - Transcription status is {Status}, skipping ⏩ remaining retries", payloadUuid, status);

    /// <summary>
    /// Logs a failed retry attempt.
    /// </summary>
    private static void LogRetryFailure(ILogger logger, int attempt, string payloadUuid) =>
        logger.LogWarning("Payload {PayloadUuid} - [FetchAttachId: RETRY {Attempt}, FAIL] Failed to fetch attachment",
            payloadUuid, attempt);
}

