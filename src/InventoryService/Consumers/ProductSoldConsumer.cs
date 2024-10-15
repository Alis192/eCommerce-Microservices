using Contracts;
using InventoryService.Data;
using InventoryService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Consumers
{
    public class ProductSoldConsumer : IConsumer<ProductSold>
    {
        private readonly InventoryDbContext _dbContext;

        public ProductSoldConsumer(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductSold> context)
        {
            // Find the inventory item by ProductId
            var inventoryItem = await _dbContext.InventoryItems
                .FirstOrDefaultAsync(i => i.ProductId == context.Message.ProductId);

            if (inventoryItem == null)
            {
                // When product is not found
                Console.WriteLine($"Product with ID {context.Message.ProductId} not found in inventory.");
                return;
            }

            // Reduce the stock quantity
            inventoryItem.StockQuantity -= context.Message.QuantitySold;

            if (inventoryItem.StockQuantity < 0)
            {
                // Prevent negative stock
                inventoryItem.StockQuantity = 0;
            }

            inventoryItem.LastUpdated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"Stock reduced for ProductId: {context.Message.ProductId}. New stock: {inventoryItem.StockQuantity}");
        }
    }
}
