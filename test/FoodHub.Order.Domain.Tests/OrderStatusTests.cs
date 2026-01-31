using FoodHub.Order.Domain.Exceptions;
using FoodHub.Order.Domain.ValueObjects;
using Xunit;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Domain.Tests;

public class OrderStatusTests
{
    [Fact]
    public void Pending_Can_Transition_To_Confirmed()
    {
        var order = CreatePendingOrder();
        order.ChangeStatus(OrderStatus.Confirmed);
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Pending_Cannot_Transition_To_Completed()
    {
        var order = CreatePendingOrder();
        Assert.Throws<DomainException>(() => order.ChangeStatus(OrderStatus.Completed));
    }

    [Fact]
    public void Completed_Is_Terminal()
    {
        var order = CreatePendingOrder();
        order.ChangeStatus(OrderStatus.Confirmed);
        order.ChangeStatus(OrderStatus.Completed);
        Assert.Throws<DomainException>(() => order.ChangeStatus(OrderStatus.Cancelled));
    }

    private static DomainOrder CreatePendingOrder()
    {
        var item = new OrderItem(Guid.NewGuid(), "Burger", 1, 9.99m);
        return new DomainOrder(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new[] { item });
    }
}
