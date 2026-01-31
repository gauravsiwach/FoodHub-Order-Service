using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Application.Mapping;

namespace FoodHub.Order.Application.Queries;

public sealed class GetOrderByIdQuery
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQuery(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return order?.ToDto();
    }
}
