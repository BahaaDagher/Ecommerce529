namespace Ecommerce529.ViewModels
{
    public class CategoryVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
