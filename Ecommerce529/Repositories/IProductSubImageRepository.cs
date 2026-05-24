namespace Ecommerce529.Repositories
{
    public interface IProductSubImageRepository :IRepository<ProductSubImage>
    {
        public void DeleteRange(IEnumerable<ProductSubImage> productSubImages);
    }
}
