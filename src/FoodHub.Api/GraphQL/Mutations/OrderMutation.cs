using FoodHub.Order.Application.Commands;
using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Domain.ValueObjects;
using HotChocolate.Authorization;
using Serilog;

namespace FoodHub.Api.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public sealed class OrderMutation
{
    [Authorize]
    public async Task<Guid> CreateOrder(CreateOrderDto input, [Service] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        Log.Information("Begin createOrder for RestaurantId {RestaurantId} and UserId {UserId}", input.RestaurantId, input.UserId);

        try
        {
            var orderId = await command.ExecuteAsync(input, cancellationToken).ConfigureAwait(false);
            Log.Information("Success createOrder with OrderId {OrderId}", orderId);
            return orderId;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failure createOrder for RestaurantId {RestaurantId} and UserId {UserId}", input.RestaurantId, input.UserId);
            throw;
        }
    }

    [Authorize]
    public async Task<bool> UpdateOrderStatus(Guid id, OrderStatus status, [Service] UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        Log.Information("Begin updateOrderStatus for OrderId {OrderId} to Status {Status}", id, status);

        try
        {
            await command.ExecuteAsync(id, status, cancellationToken).ConfigureAwait(false);
            Log.Information("Success updateOrderStatus for OrderId {OrderId} to Status {Status}", id, status);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failure updateOrderStatus for OrderId {OrderId} to Status {Status}", id, status);
            throw;
        }
    }
}
