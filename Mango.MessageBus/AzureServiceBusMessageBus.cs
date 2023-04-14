using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://bg-learn-async.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/+aXm3gFdjTOke5AolB0jyEOddrq2AWjC+ASbIEb1EQ=";
        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topicName);
            var JSonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JSonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };
            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
