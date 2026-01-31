using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Domain.ValueObjects;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Application.Mapping;

internal static class OrderMapping
{
    public static IReadOnlyList<OrderItem> ToDomainItems(this IEnumerable<CreateOrderItemDto> items)
    {
        return items.Select(item => new OrderItem(item.ItemId, item.Name, item.Quantity, item.UnitPrice))
            .ToList();
    }

    public static OrderDto ToDto(this DomainOrder order)
    {
        var items = order.Items.Select(item => new OrderItemDto(item.ItemId, item.Name, item.Quantity, item.UnitPrice))
            .ToList();

        return new OrderDto(
            order.Id,
            order.RestaurantId,
            order.UserId,
            order.TotalAmount,
            order.Status,
            order.CreatedAt,
            items);
    }
}
