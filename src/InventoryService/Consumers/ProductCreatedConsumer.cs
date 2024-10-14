using Contracts; 
using InventoryService.Data;
using InventoryService.Models;
using MassTransit;

namespace InventoryService.Consumers
{
    public class ProductCreatedConsumer : IConsumer<ProductCreated>
    {
        private readonly InventoryDbContext _dbContext;

        public ProductCreatedConsumer(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductCreated> context)
        {
            // Add new product to the inventory with initial stock
            var inventoryItem = new InventoryItem
            {
                ProductId = context.Message.Id,
                StockQuantity = 0 // Default starting stock
            };

            _dbContext.InventoryItems.Add(inventoryItem);
            await _dbContext.SaveChangesAsync();
        }
    }
}
