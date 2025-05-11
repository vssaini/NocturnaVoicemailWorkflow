using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Enums;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class ValidateToken
{
    [Function(nameof(ValidateToken))]
    public static RequestResult Run(
        [ActivityTrigger] WebhookInput input,
        FunctionContext context)
    {
        var logger = context.GetLogger(nameof(ValidateToken));
        logger.LogDebug("Checking for Validation-Token header...");

        if (input.Headers.TryGetValue("Validation-Token", out var validationTokens) && string.IsNullOrWhiteSpace(input.Payload))
        {
            var token = validationTokens.FirstOrDefault();
            logger.LogInformation("Validation token found: {Token}", token);

            return RequestResult.Create(
                RequestStatus.Success,
                "Validation token found.",
                isValidationRequest: true,
                validationToken: token);
        }

        logger.LogDebug("Validation-Token header not present and payload not empty; continuing with normal webhook processing.");
        return RequestResult.Create(
            RequestStatus.Error,
            "Validation-Token header not present and payload not empty.",
            isValidationRequest: false);
    }
}