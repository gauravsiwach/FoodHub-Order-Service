using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Domain.ValueObjects;

namespace FoodHub.Order.Application.Commands;

public sealed class UpdateOrderStatusCommand
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderStatusCommand(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task ExecuteAsync(Guid id, OrderStatus status, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (order is null)
        {
            return;
        }

        order.ChangeStatus(status);
        await _orderRepository.UpdateStatusAsync(id, order.Status, cancellationToken).ConfigureAwait(false);
    }
}
