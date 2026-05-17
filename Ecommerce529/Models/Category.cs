using Ecommerce529.CustomeValidations;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce529.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        //[MaxLength(20)]
        //[MinLength(3)]
        [MinMaxLengthAttribute(3 , 20)]
        public string Name { get; set; } = string.Empty; 
        public string? Description { get; set; }
        [Required]
        public bool Status { get; set; }

    }
}
