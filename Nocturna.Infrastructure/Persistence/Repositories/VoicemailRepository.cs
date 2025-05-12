using Dapper;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Entities;
using System.Data;

namespace Nocturna.Infrastructure.Persistence.Repositories;

public class VoicemailRepository(IDbConnectionFactory dbConnectionFactory, ILogger<VoicemailRepository> logger) : IVoicemailRepository
{
    public async Task<int> SaveWebhookPayloadAsync(string payload, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Saving webhook payload to database."); // TODO: Show bodyId in each log for tracking (brainstorm)

        var dParams = new DynamicParameters();
        dParams.Add("@Payload", payload);
        dParams.Add("@Source", "RingCentral");
        dParams.Add("@DeveloperComments", GetDevComments());
        dParams.Add("@PayloadId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition("dbo.usp_SaveWebhookPayload", dParams, cancellationToken: cancellationToken);
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

    public async Task SaveVoicemailTranscriptionAsync(int payloadId, VoicemailTranscription transcription, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Saving voicemail transcription for DB payload {DbPayloadId} to database", payloadId);

        var dParams = new DynamicParameters();
        dParams.Add("@PayloadId", payloadId);
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
        var command = new CommandDefinition("dbo.usp_SaveVoicemailTranscription", dParams, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
