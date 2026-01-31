namespace FoodHub.Order.Application.DTOs;

public sealed record CreateOrderDto(
    Guid RestaurantId,
    Guid UserId,
    List<CreateOrderItemDto> Items);
