using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Restaurant.ProductAPI.Models;

namespace Restaurant.ProductAPI.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }

        public DbSet<Product> products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 1,
                Name = "Samosa",
                Description = "An Indian Appetizer",
                Price = 5.99,
                ImageUrl = "https://bgmicrostorage.blob.core.windows.net/images/11[1].jpg",
                CategoryName = "Appetizer"

            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 2,
                Name = "Gobi Manchurian",
                Description = "An Indian Appetizer",
                Price = 12.99,
                ImageUrl = "https://bgmicrostorage.blob.core.windows.net/images/12[1].jpg",
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 3,
                Name = "Pani Pori",
                Description = "An Indian Appetizer made with delicious spices",
                Price = 7.99,
                ImageUrl = "",
                CategoryName = "Appetizer"
            });
        }
    }
}
