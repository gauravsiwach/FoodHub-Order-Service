using FoodHub.Order.Domain.Exceptions;
using FoodHub.Order.Domain.ValueObjects;
using Xunit;

namespace FoodHub.Order.Domain.Tests;

public class OrderItemTests
{
    [Fact]
    public void Constructor_Should_Throw_When_ItemId_Empty()
    {
        Assert.Throws<DomainException>(() => new OrderItem(Guid.Empty, "Burger", 1, 9.99m));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Name_Empty()
    {
        Assert.Throws<DomainException>(() => new OrderItem(Guid.NewGuid(), " ", 1, 9.99m));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Quantity_Invalid()
    {
        Assert.Throws<DomainException>(() => new OrderItem(Guid.NewGuid(), "Burger", 0, 9.99m));
    }

    [Fact]
    public void Constructor_Should_Throw_When_UnitPrice_Invalid()
    {
        Assert.Throws<DomainException>(() => new OrderItem(Guid.NewGuid(), "Burger", 1, 0m));
    }
}
