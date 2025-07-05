using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Kinodev.Functions.Services;

namespace Kinodev.Functions
{
    public class EmailSentHandler
    {
        private readonly ILogger<EmailSentHandler> _logger;

        private readonly IApiProcessor _apiProcessor;

        public EmailSentHandler(
            IApiProcessor apiProcessor,
            ILogger<EmailSentHandler> logger
            )
        {
            _logger = logger;
            _apiProcessor = apiProcessor;
        }

        [Function("EmailSentHandler")]
        public async Task Run([ServiceBusTrigger("email-sent-queue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Processing message: {MessageId}", message.MessageId);

            await _apiProcessor.ProcessEmailSentAsync(message.Body.ToString());

            _logger.LogInformation("Email sent message processed successfully.");
        }
    }
}