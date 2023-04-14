namespace Restaurant.ShoppingCartAPI.Models
{
    public class ShoppingCart
    {
        public CartHeader CartHeader { get; set; }
        public IEnumerable<CartDetail> CartDetails { get; set; }
    }
}
