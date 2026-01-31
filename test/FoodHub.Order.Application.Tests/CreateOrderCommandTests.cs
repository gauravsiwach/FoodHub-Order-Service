using FoodHub.Order.Application.Commands;
using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Domain.ValueObjects;
using Xunit;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Application.Tests;

public class CreateOrderCommandTests
{
    [Fact]
    public async Task CreateOrder_Should_Save_Order_And_Return_Id()
    {
        var repository = new InMemoryOrderRepository();
        var command = new CreateOrderCommand(repository);
        var input = new CreateOrderDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Burger", 1, 9.99m)
            });

        var id = await command.ExecuteAsync(input, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal(1, repository.SavedOrders.Count);
    }

    private sealed class InMemoryOrderRepository : IOrderRepository
    {
        public List<DomainOrder> SavedOrders { get; } = new();

        public Task AddAsync(DomainOrder order, CancellationToken cancellationToken)
        {
            SavedOrders.Add(order);
            return Task.CompletedTask;
        }

        public Task<DomainOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(SavedOrders.FirstOrDefault(order => order.Id == id));
        }

        public Task<IReadOnlyList<DomainOrder>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            IReadOnlyList<DomainOrder> result = SavedOrders.Where(order => order.UserId == userId).ToList();
            return Task.FromResult(result);
        }

        public Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken)
        {
            var order = SavedOrders.FirstOrDefault(o => o.Id == id);
            if (order is not null)
            {
                order.ChangeStatus(status);
            }

            return Task.CompletedTask;
        }
    }
}
