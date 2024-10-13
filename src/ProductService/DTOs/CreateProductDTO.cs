using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class CreateProductDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Stock Quantity must be greater than 0.")]
        public int StockQuantity { get; set; }  
    }
}
