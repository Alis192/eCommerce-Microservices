using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SearchService.Data;

namespace SearchService.Consumers
{
    public class ProductDeletedConsumer : IConsumer<ProductDeleted>
    {
        private readonly SearchDbContext _dbContext;

        public ProductDeletedConsumer(SearchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == context.Message.Id);

            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
