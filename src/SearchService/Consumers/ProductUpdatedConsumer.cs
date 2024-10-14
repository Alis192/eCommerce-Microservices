using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SearchService.Data;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class ProductUpdatedConsumer : IConsumer<ProductUpdated>
    {
        private readonly SearchDbContext _dbContext;

        public ProductUpdatedConsumer(SearchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductUpdated> context)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == context.Message.Id);

            if (product != null)
            {
                // Update product details
                product.Name = context.Message.Name;
                product.Description = context.Message.Description;
                product.Price = context.Message.Price;
                product.StockQuantity = context.Message.StockQuantity;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
