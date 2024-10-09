namespace ProductService.DTOs
{
    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int StockQuantity { get; set; }  
    }
}
