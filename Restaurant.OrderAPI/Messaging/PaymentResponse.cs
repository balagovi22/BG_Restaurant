using Mango.MessageBus;

namespace Restaurant.OrderAPI.Messaging
{
    public class PaymentResponse : BaseMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }

        public string Email { get; set; }
    }
}
