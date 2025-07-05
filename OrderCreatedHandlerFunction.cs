using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Kinodev.Functions.Services;

namespace Kinodev.Functions
{
    public class OrderCompletedHandlerFunction
    {
        private readonly ILogger<OrderCompletedHandlerFunction> _logger;

        private readonly IApiProcessor _apiProcessor;

        public OrderCompletedHandlerFunction(
            IApiProcessor apiProcessor,
            ILogger<OrderCompletedHandlerFunction> logger
            )
        {
            _logger = logger;
            _apiProcessor = apiProcessor;
        }

        [Function("OrderCompletedHandler")]
        public async Task Run([ServiceBusTrigger("order-completed-queue", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Processing message: {MessageId}", message.MessageId);
            
            await _apiProcessor.ProcessOrderCompletedAsync(message.Body.ToString());

            _logger.LogInformation("Order completed message processed successfully.");
        }
    }
}
