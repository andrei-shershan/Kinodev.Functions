using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Kinodev.Functions.Services;

namespace Kinodev.Functions
{
    public class OrderFileUrlAddedHandler
    {
        private readonly ILogger<OrderCompletedHandlerFunction> _logger;

        private readonly IApiProcessor _apiProcessor;

        public OrderFileUrlAddedHandler(
            IApiProcessor apiProcessor,
            ILogger<OrderCompletedHandlerFunction> logger
            )
        {
            _logger = logger;
            _apiProcessor = apiProcessor;
        }

        [Function("OrderFileUrlAddedHandler")]
        public async Task Run([ServiceBusTrigger("order-file-url-added-queue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Processing message: {MessageId}", message.MessageId);

            await _apiProcessor.ProcessOrderFileUrlAddedAsync(message.Body.ToString());

            _logger.LogInformation("Order file url added message processed successfully.");
        }
    }
}