using FoodHub.Order.Domain.ValueObjects;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(DomainOrder order, CancellationToken cancellationToken);
    Task<DomainOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DomainOrder>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken);
}
