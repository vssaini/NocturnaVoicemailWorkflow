using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Nocturna.Application.Abstractions;

namespace Nocturna.Presentation.Functions.Activities;

public class SavePayload(IVoicemailProcessor processor)
{
    [FunctionName(nameof(SavePayload))]
    public async Task<int> Run(
        [ActivityTrigger] string payload)
    {
        var dbPayloadId = await processor.SavePayloadAsync(payload);
        return dbPayloadId;
    }
}
