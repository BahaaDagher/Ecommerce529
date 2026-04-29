namespace Ecommerce529.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Logo { get; set; }
        public bool Status { get; set; }
    }
}
