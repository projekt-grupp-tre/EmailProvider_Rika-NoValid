
using Xunit;
using Moq;
using EmailProvider_Rika.Models;
using EmailProvider_Rika.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Tests.Interfaces;


namespace EmailProvider.Tests;

public class EmailServiceTests
{
    private readonly Mock<ILogger<EmailService>> _mockLogger;
    private readonly Mock<EmailClient> _mockEmailClient;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _mockLogger = new Mock<ILogger<EmailService>>();
        _mockEmailClient = new Mock<EmailClient>();
        _emailService = new EmailService(_mockEmailClient.Object, _mockLogger.Object);
    }

    [Fact]
    public void UnpackEmailRequest_ValidMessage_ReturnsEmailRequest()
    {
        // Arrange
        var validJson = JsonConvert.SerializeObject(new EmailRequest
        {
            To = "test@example.com",
            Subject = "Test Subject",
            HtmlBody = "<p>Test</p>",
            PlainText = "Test"
        });
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.Body).Returns(validJson);

        // Act
        var result = _emailService.UnpackEmailRequest((ServiceBusReceivedMessage)mockMessage.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@example.com", result.To);
    }

    [Fact]
    public void UnpackEmailRequest_InvalidMessage_ReturnsNull()
    {
        // Arrange
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.Body).Returns("Invalid JSON");

        // Act
        var result = _emailService.UnpackEmailRequest((ServiceBusReceivedMessage)mockMessage.Object);

        // Assert
        Assert.Null(result);
    }

    
}
