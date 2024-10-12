using MassTransit;
using SearchService.Data;
using Contracts;
using SearchService.Models;

public class ProductCreatedConsumer : IConsumer<ProductCreated>
{
    private readonly SearchDbContext _dbContext;

    public ProductCreatedConsumer(SearchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ProductCreated> context)
    {
        var product = new Product
        {
            Id = context.Message.Id,
            Name = context.Message.Name,
            Description = context.Message.Description,
            Price = context.Message.Price
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
    }
}
