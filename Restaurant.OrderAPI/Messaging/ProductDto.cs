using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.OrderAPI.Messages
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
