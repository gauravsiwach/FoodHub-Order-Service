using FoodHub.Order.Domain.Exceptions;
using FoodHub.Order.Domain.ValueObjects;
using Xunit;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Domain.Tests;

public class OrderTests
{
    [Fact]
    public void Constructor_Should_Throw_When_Items_Empty()
    {
        Assert.Throws<DomainException>(() => new DomainOrder(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Array.Empty<OrderItem>()));
    }

    [Fact]
    public void TotalAmount_Is_Sum_Of_Items()
    {
        var items = new[]
        {
            new OrderItem(Guid.NewGuid(), "Burger", 2, 10m),
            new OrderItem(Guid.NewGuid(), "Fries", 1, 5m)
        };

        var order = new DomainOrder(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), items);

        Assert.Equal(25m, order.TotalAmount);
    }

    [Fact]
    public void AddItem_Should_Recalculate_Total()
    {
        var item = new OrderItem(Guid.NewGuid(), "Burger", 1, 10m);
        var order = new DomainOrder(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new[] { item });

        order.AddItem(new OrderItem(Guid.NewGuid(), "Fries", 1, 5m));

        Assert.Equal(15m, order.TotalAmount);
    }
}
