using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Kinodev.Functions.Services;

namespace Kinodev.Functions
{
    public class OrderFileCreatedHandler
    {
        private readonly ILogger<OrderCompletedHandlerFunction> _logger;

        private readonly IApiProcessor _apiProcessor;

        public OrderFileCreatedHandler(
            IApiProcessor apiProcessor,
            ILogger<OrderCompletedHandlerFunction> logger
            )
        {
            _logger = logger;
            _apiProcessor = apiProcessor;
        }

        [Function("OrderFileCreatedHandler")]
        public async Task Run([ServiceBusTrigger("order-file-created-queue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Processing message: {MessageId}", message.MessageId);

            await _apiProcessor.ProcessOrderFileCreatedAsync(message.Body.ToString());

            _logger.LogInformation("Order file created message processed successfully.");
        }
    }
}