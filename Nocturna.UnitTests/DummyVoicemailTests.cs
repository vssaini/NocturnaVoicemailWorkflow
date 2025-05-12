using Nocturna.Domain.Models.RingCentral;
using Xunit;

namespace Nocturna.UnitTests;

public class DummyVoicemailTests
{
    [Fact]
    public async Task Dummy_ProcessVoicemailAsync_DoesNotThrow()
    {
        // Arrange
        var payload = new WebhookPayloadDto
        {
            Uuid = "8772387842463887969",
            Event = "/restapi/v1.0/account/~/extension/~/voicemail",
            Timestamp = DateTime.Parse("2025-04-24T02:25:59.740Z"),
            SubscriptionId = "b59de366-28bc-4014-8ff0-d8b90b0a7358",
            OwnerId = "2090824013",
            Body = new MessageDto
            {
                Uri = "https://...",
                Id = 2655783060012,
                To =
                [
                    new ToDto
                    {
                        PhoneNumber = "+17028447315",
                        Name = "(702) 844-7315 (Vikram Saini)",
                        Location = "Las Vegas, NV"
                    }
                ],
                From = new FromDto
                {
                    PhoneNumber = "+19515247198",
                    Name = "BANNING CA",
                    PhoneNumberInfo = new PhoneNumberInfoDto
                    {
                        CountryCode = "1",
                        NationalDestinationCode = "951",
                        SubscriberNumber = "5247198"
                    }
                },
                Type = "VoiceMail",
                CreationTime = DateTime.Parse("2025-04-24T02:25:40.000Z"),
                ReadStatus = "Unread",
                Priority = "Normal",
                Attachments =
                [
                    new AttachmentDto
                    {
                        Id = 2655783060012,
                        Uri = "https://...",
                        Type = "AudioRecording",
                        ContentType = "audio/mpeg",
                        VmDuration = 19,
                        FileName = null
                    },
                    new AttachmentDto
                    {
                        Id = 695797295012,
                        Uri = "https://...",
                        Type = "AudioTranscription",
                        ContentType = "text/plain",
                        VmDuration = 19,
                        FileName = "transcription"
                    }
                ],
                Direction = "Inbound",
                Availability = "Alive",
                MessageStatus = "Received",
                LastModifiedTime = DateTime.Parse("2025-04-24T02:25:48.549Z"),
                VmTranscriptionStatus = "Completed",
                EventType = "Create"
            }
        };

        // Act
        await Task.Run(() =>
        {
            // simulate doing something with payload
            var id = payload.Body?.Id;
            var from = payload.Body?.From?.PhoneNumber;
            var to = payload.Body?.To?[0]?.PhoneNumber;
        });

        // Assert (no-op)
    }
}
