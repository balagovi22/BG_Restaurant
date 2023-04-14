using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using PaymentProcessor;
using Restaurant.ProcessPaymentAPI.Messages;
using Restaurant.ProcessPaymentAPI.Models;

using System.Text;

namespace Restaurant.ProcessPaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;
        private readonly string serviceBusConnectionString;
        private readonly string processPaymentTopicName;
        private readonly string updateOrderPaymentTopicName;
        private readonly string paymentSubscription;

        private ServiceBusProcessor paymentProcessor;
        ServiceBusReceiver receiver;

        public AzureServiceBusConsumer(IConfiguration configuration, 
                                        IMessageBus messageBus,
                                        IProcessPayment processPayment)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _processPayment = processPayment;

            //serviceBusConnectionString = _configuration.GetValue<string>("AzureServiceBus:ConnectionString");
            //checkoutMessageTopicName = _configuration.GetValue<string>("AzureServiceBus:CheckoutMessageTopicName");
            //checkoutMessageSubscription = _configuration.GetValue<string>("AzureServiceBus:CheckoutMessageSubscriptionName");

            serviceBusConnectionString = _configuration.GetValue<string>("ConnectionString");
            processPaymentTopicName = _configuration.GetValue<string>("ProcessPaymentTopic");
            paymentSubscription = _configuration.GetValue<string>("PaymentSubscription");
            updateOrderPaymentTopicName = _configuration.GetValue<string>("OrderUpdatePaymentTopic");

            var client= new ServiceBusClient(serviceBusConnectionString);
            // create a receiver for our subscription that we can use to receive the message
            //receiver = client.CreateReceiver("checkoutordermessagetopic", "checkOutOrderSubscription");
            // the received message is a different type as it contains some service set properties
             paymentProcessor = client.CreateProcessor(processPaymentTopicName, paymentSubscription);
            //paymentProcessor = client.CreateProcessor("checkoutordermessagetopic", "checkOutOrderSubscription");
        }
        public async Task Start()
        {
            paymentProcessor.ProcessMessageAsync += ProcessPayment;
            paymentProcessor.ProcessErrorAsync += ErrorHandler;
            await paymentProcessor.StartProcessingAsync();
        }
        async Task ProcessPayment(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PaymentRequestMessage paymentRequest = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);
            var result = _processPayment.ProcessPayment();

            PaymentResponse paymentResponse = new()
            {
                Status = result,
                OrderId = paymentRequest.OrderId,
                Email = paymentRequest.Email
            };
            try
             {
                    await _messageBus.PublishMessage(paymentResponse, updateOrderPaymentTopicName);
                    await args.CompleteMessageAsync(args.Message);
             }
             catch(Exception ex)
             {
                    Console.WriteLine(ex.ToString());
             }
        }

        public async Task Stop()
        {
          await paymentProcessor.StopProcessingAsync();
          await paymentProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }


       
    }
}
