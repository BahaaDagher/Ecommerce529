namespace Ecommerce529.ViewModels
{
    public class BrandVM
    {
        public IEnumerable<Brand> Brands { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
