using Azure.Messaging;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IntegrationApps
{
    public class ServiceBusReceivedMessageFunctions(ILogger<ServiceBusReceivedMessageFunctions> logger)
    {
        [Function(nameof(ServiceBusReceivedMessageFunction))]
        [CosmosDBOutput("%CosmosDatabaseName%", "events", Connection = "CosmosDBConnection")]
        public object ServiceBusReceivedMessageFunction(
            [ServiceBusTrigger("fileupload-dev", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            logger.LogInformation("Message ID: {id}", message.MessageId);
            logger.LogInformation("Message Body: {body}", message.Body);
            logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
            try
            {
                var cloudEvent = CloudEvent.Parse(message.Body);
                return cloudEvent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
