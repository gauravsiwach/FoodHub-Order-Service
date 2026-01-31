namespace FoodHub.Order.Infrastructure.Persistence.Cosmos;

public sealed class CosmosOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public Dictionary<string, CosmosContainerOptions> Containers { get; set; } = new();
}

public sealed class CosmosContainerOptions
{
    public string Name { get; set; } = string.Empty;
}
