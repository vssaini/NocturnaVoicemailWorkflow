using Moq;
using Nocturna.Application.Abstractions.RingCentral;
using Nocturna.Application.UseCases;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Models;
using Xunit;

namespace Nocturna.UnitTests;

public class ProcessVoicemailUseCaseTests
{
    private readonly Mock<IRingCentralService> _ringCentralServiceMock;
    private readonly Mock<IVoicemailRepository> _voicemailRepositoryMock;
    private readonly VoicemailService _useCase;

    public ProcessVoicemailUseCaseTests()
    {
        _ringCentralServiceMock = new Mock<IRingCentralService>();
        _voicemailRepositoryMock = new Mock<IVoicemailRepository>();
        _useCase = new ProcessVoicemailUseCase(_ringCentralServiceMock.Object, _voicemailRepositoryMock.Object);
    }

    [Fact]
    public async Task ProcessVoicemailAsync_ValidPayload_SavesVoicemail()
    {
        // Arrange
        var payload = new WebhookPayloadDto
        {
            Uuid = "8772387842463887969",
            Event = "/restapi/v1.0/account/~/extension/~/voicemail",
            Timestamp = DateTime.Parse("2025-04-24T02:25:59.740Z"),
            SubscriptionId = "b59de366-28bc-4014-8ff0-d8b90b0a7358",
            OwnerId = "2090824013",
            Body = new MessageBodyDto
            {
                Uri = "https://api.ringcentral.com/restapi/v1.0/account/63123630012/extension/2090824013/message-store/2655783060012",
                Id = 2655783060012,
                To = new[]
                {
                    new ToDto
                    {
                        PhoneNumber = "+17028447315",
                        Name = "(702) 844-7315 (Vikram Saini)",
                        Location = "Las Vegas, NV"
                    }
                },
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
                Attachments = new[]
                {
                    new AttachmentDto
                    {
                        Id = 2655783060012,
                        Uri = "https://media.ringcentral.com/restapi/v1.0/account/63123630012/extension/2090824013/message-store/2655783060012/content/2655783060012",
                        Type = "AudioRecording",
                        ContentType = "audio/mpeg",
                        VmDuration = 19,
                        FileName = null
                    },
                    new AttachmentDto
                    {
                        Id = 695797295012,
                        Uri = "https://media.ringcentral.com/restapi/v1.0/account/63123630012/extension/2090824013/message-store/2655783060012/content/695797295012",
                        Type = "AudioTranscription",
                        ContentType = "text/plain",
                        VmDuration = 19,
                        FileName = "transcription"
                    }
                },
                Direction = "Inbound",
                Availability = "Alive",
                MessageStatus = "Received",
                LastModifiedTime = DateTime.Parse("2025-04-24T02:25:48.549Z"),
                VmTranscriptionStatus = "Completed",
                EventType = "Create"
            }
        };
        _ringCentralServiceMock.Setup(x => x.GetTranscriptionAsync("2655783060012", "695797295012")).ReturnsAsync("Test transcription");

        // Act
        await _useCase.ProcessVoicemailAsync(payload);

        // Assert
        _voicemailRepositoryMock.Verify(x => x.SaveAsync(It.Is<VoicemailMessage>(vm =>
            vm.Id == 2655783060012 &&
            vm.Transcription == "Test transcription" &&
            vm.From.Number == "+19515247198" &&
            vm.From.Name == "BANNING CA" &&
            vm.From.CountryCode == "1" &&
            vm.To.Number == "+17028447315" &&
            vm.To.Name == "(702) 844-7315 (Vikram Saini)" &&
            vm.CreationTime == DateTime.Parse("2025-04-24T02:25:40.000Z")
        )), Times.Once());
    }

    [Fact]
    public async Task ProcessVoicemailAsync_InvalidEvent_ThrowsException()
    {
        // Arrange
        var payload = new WebhookPayloadDto { Event = "invalid" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ProcessVoicemailAsync(payload));
    }
}