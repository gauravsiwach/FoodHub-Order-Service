using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FoodHub.Order.Infrastructure.Persistence.Cosmos;

public sealed class CosmosContext
{
    private readonly CosmosOptions _options;
    private readonly CosmosClient _client;

    public CosmosContext(CosmosClient client, IOptions<CosmosOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public Container Orders
    {
        get
        {
            if (!_options.Containers.TryGetValue("Order", out var containerOptions))
            {
                throw new InvalidOperationException("Cosmos container configuration for 'Order' was not found.");
            }

            return _client.GetContainer(_options.DatabaseName, containerOptions.Name);
        }
    }
}
