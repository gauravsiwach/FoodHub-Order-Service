using FoodHub.Order.Domain.ValueObjects;

namespace FoodHub.Order.Application.DTOs;

public sealed record OrderDto(
    Guid Id,
    Guid RestaurantId,
    Guid UserId,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    List<OrderItemDto> Items);
