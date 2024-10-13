using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity can't be negative.")]
        public int StockQuantity { get; set; }
    }
}
