using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using Restaurant.OrderAPI.Messages;
using Restaurant.OrderAPI.Models;
using Restaurant.OrderAPI.Repository;
using System.Text;

namespace Restaurant.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        private readonly string serviceBusConnectionString;
        private readonly string checkoutMessageTopicName;
        private readonly string processPaymentTopicName;
        private readonly string checkoutMessageSubscription;
        private readonly string paymentSubscription;
        private readonly string orderPaymentResultTopic;
        private readonly string orderPaymentResultSubscription;
        private readonly string checkoutMessageQueue;

        private ServiceBusProcessor checkOutProcessor;
        private ServiceBusProcessor updateOrderPaymentStatusProcessor;
        ServiceBusReceiver receiver;

        public AzureServiceBusConsumer(OrderRepository orderRepository, 
                                        IConfiguration configuration, 
                                        IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;

            //serviceBusConnectionString = _configuration.GetValue<string>("AzureServiceBus:ConnectionString");
            //checkoutMessageTopicName = _configuration.GetValue<string>("AzureServiceBus:CheckoutMessageTopicName");
            //checkoutMessageSubscription = _configuration.GetValue<string>("AzureServiceBus:CheckoutMessageSubscriptionName");

            serviceBusConnectionString = _configuration.GetValue<string>("ConnectionString");
            checkoutMessageTopicName = _configuration.GetValue<string>("CheckoutMessageTopicName");
            checkoutMessageSubscription = _configuration.GetValue<string>("CheckoutMessageSubscriptionName");
            processPaymentTopicName = _configuration.GetValue<string>("ProcessPaymentTopic");
            paymentSubscription = _configuration.GetValue<string>("PaymentSubscription");
            orderPaymentResultTopic = _configuration.GetValue<string>("OrderPaymentResultTopic");
            orderPaymentResultSubscription = _configuration.GetValue<string>("OrderPaymentResultSubscription");
            checkoutMessageQueue = _configuration.GetValue<string>("CheckoutMessageQueue");
            var client= new ServiceBusClient(serviceBusConnectionString);
            //Just for testing queue, replace queue name
            checkOutProcessor = client.CreateProcessor(checkoutMessageQueue.Trim());
            //This is for Topics
            //checkOutProcessor = client.CreateProcessor(checkoutMessageTopicName.Trim(), checkoutMessageSubscription.Trim());
            updateOrderPaymentStatusProcessor = client.CreateProcessor("orderupdatepaymentresulttopic", "updateorderpaymentsubscription");
            //updateOrderPaymentStatusProcessor = client.CreateProcessor(orderPaymentResultTopic, orderPaymentResultSubscription);
            //checkOutProcessor = client.CreateProcessor("checkoutordermessagetopic", "checkOutOrderSubscription");
        }
        public async Task Start()
        {
            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();

            updateOrderPaymentStatusProcessor.ProcessMessageAsync += OnPaymentPosted;
            updateOrderPaymentStatusProcessor.ProcessErrorAsync += PaymentErrorHandler;
            await updateOrderPaymentStatusProcessor.StartProcessingAsync();
        }
        async Task OnPaymentPosted(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            PaymentResponse orderPaymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(body);

            if (orderPaymentResponse != null)
            {
                await _orderRepository.UpdateOrderPaymentStatus(orderPaymentResponse.OrderId, orderPaymentResponse.Status);
                await args.CompleteMessageAsync(args.Message);
            }
        }

        async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CheckoutDto checkoutDto = JsonConvert.DeserializeObject<CheckoutDto>(body);
            if (checkoutDto != null)
            {
                OrderHeader orderHeader = new()
                {
                    UserId = checkoutDto.UserId,
                    FirstName = checkoutDto.FirstName,
                    LastName = checkoutDto.LastName,
                    OrderDetails = new List<OrderDetail>(),
                    CardNumber = checkoutDto.CardNumber,
                    CouponCode = checkoutDto.CouponCode,
                    CVV = checkoutDto.CVV,
                    DiscountTotal = checkoutDto.DiscountTotal,
                    Email = checkoutDto.Email,
                    ExpiryMonthYear = checkoutDto.ExpiryMonthYear,
                    OrderPlacedTime = DateTime.Now,
                    PaymentStatus = false,
                    Phone = checkoutDto.Phone,
                    PickupDateTime = checkoutDto.PickupDateTime,
                };
                foreach (var details in checkoutDto.CartDetails)
                {
                    OrderDetail orderDetails = new()
                    {
                        ProductId = details.ProductId,
                        ProductName = details.Product.Name,
                        ProductPrice = details.Product.Price,
                        Count = details.Count
                    };
                    orderHeader.CartTotalItems += details.Count;
                    orderHeader.OrderDetails.Add(orderDetails);
                }

                await _orderRepository.AddOrder(orderHeader);

                PaymentRequestMessage paymentRequest = new()
                {
                    Name = orderHeader.FirstName + " " + orderHeader.LastName,
                    CardNumber = orderHeader.CardNumber,
                    CVV = orderHeader.CVV,
                    ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                    OrderId = orderHeader.OrderHeaderId,
                    OrderTotal = orderHeader.OrderTotal
                };

                try
                {
                    await _messageBus.PublishMessage(paymentRequest, processPaymentTopicName);
                    await args.CompleteMessageAsync(args.Message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            };
        }

        public async Task Stop()
        {
          await checkOutProcessor.StopProcessingAsync();
          await checkOutProcessor.DisposeAsync();

          await updateOrderPaymentStatusProcessor.StopProcessingAsync();
          await updateOrderPaymentStatusProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        Task PaymentErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            await args.CompleteMessageAsync(args.Message);
        }

       
    }
}
