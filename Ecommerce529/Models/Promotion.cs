namespace Ecommerce529.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Code { get; set; }
        public  decimal Discount { get; set; }
        public int MaxUsage { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo {  get; set; }
    }
}
