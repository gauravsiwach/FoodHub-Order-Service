using System.Net;
using FoodHub.Order.Application.Interfaces;
using FoodHub.Order.Domain.ValueObjects;
using FoodHub.Order.Infrastructure.Persistence.Cosmos;
using Microsoft.Azure.Cosmos;
using DomainOrder = FoodHub.Order.Domain.Entities.Order;

namespace FoodHub.Order.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly Container _container;

    public OrderRepository(CosmosContext context)
    {
        _container = context.Orders;
    }

    public async Task AddAsync(DomainOrder order, CancellationToken cancellationToken)
    {
        var document = ToDocument(order);
        await _container.CreateItemAsync(document, new PartitionKey(document.Id), cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<DomainOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<OrderDocument>(
                id.ToString(),
                new PartitionKey(id.ToString()),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return ToDomain(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<DomainOrder>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId.ToString());

        var results = new List<DomainOrder>();
        using var iterator = _container.GetItemQueryIterator<OrderDocument>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            results.AddRange(response.Select(ToDomain));
        }

        return results;
    }

    public async Task UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<OrderDocument>(
                id.ToString(),
                new PartitionKey(id.ToString()),
                cancellationToken: cancellationToken).ConfigureAwait(false);

            response.Resource.Status = status.ToString();

            await _container.ReplaceItemAsync(
                response.Resource,
                response.Resource.Id,
                new PartitionKey(response.Resource.Id),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }

    private static OrderDocument ToDocument(DomainOrder order)
    {
        return new OrderDocument
        {
            Id = order.Id.ToString(),
            RestaurantId = order.RestaurantId.ToString(),
            UserId = order.UserId.ToString(),
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(item => new OrderItemDocument
            {
                ItemId = item.ItemId.ToString(),
                Name = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    private static DomainOrder ToDomain(OrderDocument document)
    {
        var items = document.Items.Select(item => new OrderItem(
            Guid.Parse(item.ItemId),
            item.Name,
            item.Quantity,
            item.UnitPrice));

        var status = Enum.TryParse<OrderStatus>(document.Status, out var parsed)
            ? parsed
            : OrderStatus.Pending;

        return new DomainOrder(
            Guid.Parse(document.Id),
            Guid.Parse(document.RestaurantId),
            Guid.Parse(document.UserId),
            items,
            document.CreatedAt,
            status);
    }
}
