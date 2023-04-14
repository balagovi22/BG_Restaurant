using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.OrderAPI.Database;
using Restaurant.OrderAPI.Models;

namespace Restaurant.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
       private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid)
        {
            await using var _db = new ApplicationDbContext(_dbContext);
            var orderHeaderFromDB = await _db.OrderHeaders.FirstOrDefaultAsync(u=>u.OrderHeaderId== orderHeaderId);
            if (orderHeaderFromDB != null)
            {
                orderHeaderFromDB.PaymentStatus= isPaid;
                await _db.SaveChangesAsync();
            }
        }
    }
}
