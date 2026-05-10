namespace Ecommerce529.ViewModels
{
    public class ProductVM
    {
        public IEnumerable<Product> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
