namespace Ecommerce529.Repositories
{
    public interface IProductColorRepository :IRepository<ProductColor>
    {
        public void DeleteRange(IEnumerable<ProductColor> productColors);
    }
}
