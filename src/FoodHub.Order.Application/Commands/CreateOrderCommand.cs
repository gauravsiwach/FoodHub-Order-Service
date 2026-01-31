using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Application.Mapping;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Application.Commands;

public sealed class CreateOrderCommand
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommand(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Guid> ExecuteAsync(CreateOrderDto input, CancellationToken cancellationToken)
    {
        var items = input.Items.ToDomainItems();
        var order = new DomainOrder(Guid.NewGuid(), input.RestaurantId, input.UserId, items);

        await _orderRepository.AddAsync(order, cancellationToken).ConfigureAwait(false);

        return order.Id;
    }
}
