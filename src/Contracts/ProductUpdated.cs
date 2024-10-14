namespace Contracts
{
    public record ProductUpdated(Guid Id, string Name, string Description, double Price, int StockQuantity);
}
