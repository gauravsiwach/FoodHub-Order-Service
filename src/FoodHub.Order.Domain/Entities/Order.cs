using FoodHub.Order.Domain.Exceptions;
using FoodHub.Order.Domain.ValueObjects;

namespace FoodHub.Order.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items;

    public Guid Id { get; }
    public Guid RestaurantId { get; }
    public Guid UserId { get; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public Order(
        Guid id,
        Guid restaurantId,
        Guid userId,
        IEnumerable<OrderItem> items,
        DateTime? createdAt = null,
        OrderStatus status = OrderStatus.Pending)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Id cannot be empty.");
        }

        if (restaurantId == Guid.Empty)
        {
            throw new DomainException("RestaurantId cannot be empty.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("UserId cannot be empty.");
        }

        if (items is null)
        {
            throw new DomainException("Items cannot be null.");
        }

        _items = new List<OrderItem>(items);
        if (_items.Count == 0)
        {
            throw new DomainException("Items cannot be empty.");
        }

        Id = id;
        RestaurantId = restaurantId;
        UserId = userId;
        Status = status;
        CreatedAt = NormalizeCreatedAt(createdAt);
        RecalculateTotal();

        if (TotalAmount <= 0)
        {
            throw new DomainException("TotalAmount must be greater than zero.");
        }
    }

    public void AddItem(OrderItem item)
    {
        if (item is null)
        {
            throw new DomainException("Item cannot be null.");
        }

        _items.Add(item);
        RecalculateTotal();

        if (TotalAmount <= 0)
        {
            throw new DomainException("TotalAmount must be greater than zero.");
        }
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == newStatus)
        {
            return;
        }

        var isValid = Status switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,
            OrderStatus.Confirmed => newStatus is OrderStatus.Completed or OrderStatus.Cancelled,
            OrderStatus.Completed => false,
            OrderStatus.Cancelled => false,
            _ => false
        };

        if (!isValid)
        {
            throw new DomainException($"Invalid status transition from {Status} to {newStatus}.");
        }

        Status = newStatus;
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(item => item.Quantity * item.UnitPrice);
    }

    private static DateTime NormalizeCreatedAt(DateTime? createdAt)
    {
        if (!createdAt.HasValue || createdAt.Value == default)
        {
            return DateTime.UtcNow;
        }

        return createdAt.Value.Kind == DateTimeKind.Utc
            ? createdAt.Value
            : DateTime.SpecifyKind(createdAt.Value, DateTimeKind.Utc);
    }
}
