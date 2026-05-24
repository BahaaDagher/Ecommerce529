namespace Ecommerce529.Repositories
{
    public class ProductColorRepository : Repository<ProductColor> , IProductColorRepository
    {
        public void DeleteRange(IEnumerable<ProductColor> productColors )
        {
            _context.ProductColors.RemoveRange(productColors);
        }
    }
}
