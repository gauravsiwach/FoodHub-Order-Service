using FoodHub.Order.Application.DTOs;
using FoodHub.Order.Application.Queries;
using HotChocolate.Authorization;
using Serilog;

namespace FoodHub.Api.GraphQL.Queries;

[ExtendObjectType("Query")]
public sealed class OrderQuery
{
    [Authorize]
    public async Task<OrderDto?> GetOrderById(Guid id, [Service] GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        Log.Information("Begin getOrderById for OrderId {OrderId}", id);

        try
        {
            var result = await query.ExecuteAsync(id, cancellationToken).ConfigureAwait(false);
            Log.Information("Success getOrderById for OrderId {OrderId}", id);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failure getOrderById for OrderId {OrderId}", id);
            throw;
        }
    }

    [Authorize]
    public async Task<IReadOnlyList<OrderDto>> GetOrdersByUser(Guid userId, [Service] GetOrdersByUserQuery query, CancellationToken cancellationToken)
    {
        Log.Information("Begin getOrdersByUser for UserId {UserId}", userId);

        try
        {
            var result = await query.ExecuteAsync(userId, cancellationToken).ConfigureAwait(false);
            Log.Information("Success getOrdersByUser for UserId {UserId}", userId);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failure getOrdersByUser for UserId {UserId}", userId);
            throw;
        }
    }
}
