namespace Restaurant.Web
{
    public static class SD
    {
        public static string ProductAPIBaseURL { get; set; } = "";
        public static string ShoppingCartAPIBaseURL { get; set; } = "";

        public static string DiscountAPIBaseURL { get; set; } = "";
        public enum ApiType
        {
            GET,POST, PUT, DELETE
        }
    }
}
