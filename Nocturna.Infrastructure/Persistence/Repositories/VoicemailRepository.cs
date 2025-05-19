using Dapper;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Entities;
using Nocturna.Domain.Models;
using System.Data;

namespace Nocturna.Infrastructure.Persistence.Repositories;

public class VoicemailRepository(IDbConnectionFactory dbConnectionFactory, ILogger<VoicemailRepository> logger) : IVoicemailRepository
{
    public async Task<int> SaveWebhookPayloadAsync(ActivityContext<string> context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Payload {PayloadUuid} - Saving webhook payload to database", context.PayloadUuid);

        var dParams = new DynamicParameters();
        dParams.Add("@Payload", context.Data);
        dParams.Add("@Source", "RingCentral");
        dParams.Add("@DeveloperComments", GetDevComments());
        dParams.Add("@PayloadId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition("comm.usp_SaveWebhookPayload", dParams, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);

        return dParams.Get<int>("@PayloadId");
    }

    private static string GetDevComments()
    {
        var devComments = "None";
#if DEBUG
        devComments = "Local DEBUG mode testing";
#endif

        return devComments;
    }

    public async Task SaveVoicemailTranscriptionAsync(int savedPayloadId, ActivityContext<VoicemailTranscription> context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Payload {PayloadUuid} - Saving voicemail transcription to database.", context.PayloadUuid);

        var transcription = context.Data;

        var dParams = new DynamicParameters();
        dParams.Add("@PayloadId", savedPayloadId);
        dParams.Add("@RingCentralUuid", transcription.RingCentralUuid);
        dParams.Add("@CallDateTime", transcription.CallDateTime);
        dParams.Add("@FromPhoneNumber", transcription.From.Number);
        dParams.Add("@FromName", transcription.From.Name);
        dParams.Add("@ToPhoneNumber", transcription.To.Number);
        dParams.Add("@ToName", transcription.To.Name);
        dParams.Add("@TranscriptionText", transcription.TranscriptionText);
        dParams.Add("@AudioTranscriptionUri", transcription.AudioTranscriptionUri);
        dParams.Add("@AudioRecordingUri", transcription.AudioRecordingUri);

        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition("comm.usp_SaveVoicemailTranscription", dParams, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
