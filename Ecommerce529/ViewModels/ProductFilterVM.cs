namespace Ecommerce529.ViewModels
{
    public class ProductFilterVM
    {
        public string ProductName { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int CategoryId{ get; set; }
        public int BrandId{ get; set; }
        public bool IsLowQuantity{ get; set; }
    }
}
