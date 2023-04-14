using Mango.MessageBus;

namespace Restaurant.ProcessPaymentAPI.Models
{
    public class PaymentResponse : BaseMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }

        public string Email { get; set; }
    }
}
