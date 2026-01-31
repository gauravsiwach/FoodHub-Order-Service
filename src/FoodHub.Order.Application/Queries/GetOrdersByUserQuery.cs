using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Application.Mapping;

namespace FoodHub.Order.Application.Queries;

public sealed class GetOrdersByUserQuery
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersByUserQuery(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
        return orders.Select(order => order.ToDto()).ToList();
    }
}
