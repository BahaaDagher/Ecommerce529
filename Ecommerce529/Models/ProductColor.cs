using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce529.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
