using FoodHub.Order.Domain.Exceptions;

namespace FoodHub.Order.Domain.ValueObjects;

public sealed record OrderItem
{
    public Guid ItemId { get; }
    public string Name { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public OrderItem(Guid itemId, string name, int quantity, decimal unitPrice)
    {
        if (itemId == Guid.Empty)
        {
            throw new DomainException("ItemId cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Name cannot be empty.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainException("UnitPrice must be greater than zero.");
        }

        ItemId = itemId;
        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
