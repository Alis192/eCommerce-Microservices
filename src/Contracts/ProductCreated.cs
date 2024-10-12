namespace Contracts
{
    public record ProductCreated(Guid Id, string Name, string Description, double Price, int StockQuantity);
}
