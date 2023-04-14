using Restaurant.OrderAPI.Models;

namespace Restaurant.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid);
    }
}
