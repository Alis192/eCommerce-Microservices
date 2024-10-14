namespace SearchService.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        
        //TO DO: Need to be add migration
        public int StockQuantity { get; set; }
    }
}
