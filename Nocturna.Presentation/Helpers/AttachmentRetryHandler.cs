using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Functions.Activities;

namespace Nocturna.Presentation.Helpers;

public static class AttachmentRetryHandler
{
    public static async Task<long?> FetchWithRetryAsync(
        TaskOrchestrationContext context,
        WebhookPayloadDto payloadDto)
    {
        var logger = context.CreateReplaySafeLogger(typeof(AttachmentRetryHandler));

        var initialAttachmentId = await context.CallActivityAsync<long?>(nameof(FetchTranAttachId), payloadDto);
        if (initialAttachmentId.HasValue)
            return initialAttachmentId;

        logger.LogWarning("Attachment Id not found for payload {PayloadUuid}. Starting retry sequence...",
            payloadDto.Uuid);

        var retryDelays = new[]
        {
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(45),
            TimeSpan.FromSeconds(60),
            TimeSpan.FromSeconds(120),
        };

        return await RetryAsync(context, payloadDto, retryDelays, logger);
    }

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

    private static async Task<long?> HandleRetryAttemptAsync(
        TaskOrchestrationContext context,
        WebhookPayloadDto payloadDto,
        ILogger logger,
        int attempt,
        TimeSpan delay)
    {
        LogRetryWaiting(logger, attempt, delay, payloadDto.Uuid);

        await context.CreateTimer(context.CurrentUtcDateTime.Add(delay), CancellationToken.None);

        var attachmentId = await context.CallActivityAsync<long?>(nameof(FetchTranAttachIdFromRingCentral), payloadDto);

        if (attachmentId.HasValue)
        {
            LogRetrySuccess(logger, attempt, payloadDto.Uuid);
            return attachmentId;
        }

        LogRetryFailure(logger, attempt, payloadDto.Uuid);
        return null;
    }

    private static void LogRetryWaiting(ILogger logger, int attempt, TimeSpan delay, string payloadUuid) =>
        logger.LogInformation(
            "[RETRY {Attempt}] - Waiting {DelaySeconds}s to fetch attachment Id for payload {PayloadUuid}.",
            attempt, delay.TotalSeconds, payloadUuid);

    private static void LogRetrySuccess(ILogger logger, int attempt, string payloadUuid) =>
        logger.LogInformation("[RETRY {Attempt}] - Successfully fetched attachment Id for payload {PayloadUuid}.",
            attempt, payloadUuid);

    private static void LogRetryFailure(ILogger logger, int attempt, string payloadUuid) =>
        logger.LogWarning("[RETRY {Attempt}] - Attempt failed for payload {PayloadUuid}.",
            attempt, payloadUuid);
}
