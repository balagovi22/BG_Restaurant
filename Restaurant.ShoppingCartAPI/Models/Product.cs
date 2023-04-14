using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.ShoppingCartAPI.Models
{
    public class Product
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }


    }
}
