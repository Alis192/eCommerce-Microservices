namespace InventoryService.Models
{
    public class InventoryItem
    {
        public Guid ProductId { get; set; } // Tied to Product
        public int StockQuantity { get; set; }
    }
}
