namespace FoodHub.Order.Application.DTOs;

public sealed record OrderItemDto(
    Guid ItemId,
    string Name,
    int Quantity,
    decimal UnitPrice);
