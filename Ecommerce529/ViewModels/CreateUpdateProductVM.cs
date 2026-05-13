namespace Ecommerce529.ViewModels
{
    public class CreateUpdateProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<ProductSubImage> ProductSubImages { get; set; }
        public IEnumerable<ProductColor> ProductColors { get; set; }

    }
}
