namespace Ecommerce529.ViewModels
{
    public class OrderVM
    {
        public IEnumerable<Order> Orders { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
