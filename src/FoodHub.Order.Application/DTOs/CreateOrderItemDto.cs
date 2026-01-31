namespace FoodHub.Order.Application.DTOs;

public sealed record CreateOrderItemDto(
    Guid ItemId,
    string Name,
    int Quantity,
    decimal UnitPrice);
