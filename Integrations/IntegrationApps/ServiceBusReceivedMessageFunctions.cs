using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace IntegrationApps
{
    public class ServiceBusReceivedMessageFunctions
    {
        private readonly ILogger<ServiceBusReceivedMessageFunctions> _logger;

        public ServiceBusReceivedMessageFunctions(ILogger<ServiceBusReceivedMessageFunctions> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ServiceBusReceivedMessageFunction))]
        public string ServiceBusReceivedMessageFunction(
            [ServiceBusTrigger("fileupload-dev", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            var outputMessage = $"Output message created at {DateTime.Now}";
            return outputMessage;
        }
    }
}
