
using Azure.Messaging.ServiceBus;
using EmailProvider_Rika.Models;

namespace EmailProvider_Rika.Services;

public interface IEmailService
{
    EmailRequest UnpackEmailRequest(ServiceBusReceivedMessage message);

    bool SendEmail(EmailRequest emailRequest);
}
