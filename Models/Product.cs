using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiEcommerce.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string SKU { get; set; } = string.Empty; // Stock Keeping Unit, un identificador único para el producto ejemplo: "CAMISA-001"

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un valor positivo")]
        public int Stock { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; } = null;

        // Relación con la categoría
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public required Category Category { get; set; }
    }
}
