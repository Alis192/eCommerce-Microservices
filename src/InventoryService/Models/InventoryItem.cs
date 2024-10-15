namespace InventoryService.Models
{
    public class InventoryItem
    {
        public Guid ProductId { get; set; } // Tied to Product
        public Guid InventoryItemId { get; set; }
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
