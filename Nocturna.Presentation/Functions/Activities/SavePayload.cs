using Microsoft.Azure.Functions.Worker;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

namespace Nocturna.Presentation.Functions.Activities;

public class SavePayload(IVoicemailProcessor processor)
{
    [Function(nameof(SavePayload))]
    public async Task<int> Run(
        [ActivityTrigger] ActivityContext<string> context)
    {
        var dbPayloadId = await processor.SavePayloadAsync(context);

        if (dbPayloadId <= 0)
            throw new InvalidOperationException("Failed to save payload to database");

        return dbPayloadId;
    }
}
