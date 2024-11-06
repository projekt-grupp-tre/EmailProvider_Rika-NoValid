using Azure.Messaging.ServiceBus;
using EmailProvider.Tests.Interfaces;
using EmailProvider_Rika.Functions;
using EmailProvider_Rika.Models;
using EmailProvider_Rika.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;

public class EmailSenderTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<EmailSender>> _mockLogger;
    private readonly EmailSender _emailSender;

    public EmailSenderTests()
    {
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<EmailSender>>();
        _emailSender = new EmailSender(_mockLogger.Object, _mockEmailService.Object);
    }

    [Fact]
    public async Task Run_ValidEmailRequest_CompletesMessage()
    {
        // Arrange
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.Body).Returns("{\"To\":\"test@example.com\",\"Subject\":\"Test\",\"HtmlBody\":\"<p>Test</p>\",\"PlainText\":\"Test\"}");
        var mockMessageActions = new Mock<IMessageActions>();

        var emailRequest = new EmailRequest
        {
            To = "test@example.com",
            Subject = "Test Subject",
            HtmlBody = "<p>Test</p>",
            PlainText = "Test"
        };

        _mockEmailService.Setup(service => service.UnpackEmailRequest((ServiceBusReceivedMessage)mockMessage.Object)).Returns(emailRequest);
        _mockEmailService.Setup(service => service.SendEmail(emailRequest)).Returns(true);

        // Act
        await _emailSender.Run((ServiceBusReceivedMessage)mockMessage.Object, (ServiceBusMessageActions)mockMessageActions.Object);

        // Assert
        mockMessageActions.Verify(actions => actions.CompleteMessageAsync(mockMessage.Object), Times.Once);
    }

    [Fact]
    public async Task Run_InvalidEmailRequest_DoesNotCompleteMessage()
    {
        // Arrange
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.Body).Returns("Invalid JSON");
        var mockMessageActions = new Mock<IMessageActions>();

        _mockEmailService.Setup(service => service.UnpackEmailRequest((ServiceBusReceivedMessage)mockMessage.Object)).Returns((EmailRequest)null);

        // Act
        await _emailSender.Run((ServiceBusReceivedMessage)mockMessage.Object, (ServiceBusMessageActions)mockMessageActions.Object);

        // Assert
        mockMessageActions.Verify(actions => actions.CompleteMessageAsync(It.IsAny<IMessage>()), Times.Never);
    }
}