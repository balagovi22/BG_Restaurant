namespace Restaurant.Web.Models.Dto
{
    public class ShoppingCartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailDto> CartDetails { get; set; }
    }
}
