using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce529.Models
{
    public class ProductSubImage
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
