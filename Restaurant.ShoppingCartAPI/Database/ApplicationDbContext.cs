using Microsoft.EntityFrameworkCore;
using Restaurant.ShoppingCartAPI.Models;

namespace Restaurant.ProductAPI.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartHeader> CartHeader { get; set; }
        public DbSet<CartDetail> CartDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
