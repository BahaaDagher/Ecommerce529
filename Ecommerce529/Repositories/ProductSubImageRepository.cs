namespace Ecommerce529.Repositories
{
    public class ProductSubImageRepository :Repository<ProductSubImage> , IProductSubImageRepository
    {
        public ProductSubImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(IEnumerable<ProductSubImage> productSubImages)
        {
            _context.ProductSubImages.RemoveRange(productSubImages); 
        }
    }
}
