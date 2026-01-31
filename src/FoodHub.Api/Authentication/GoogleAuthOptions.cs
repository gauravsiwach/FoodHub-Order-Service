namespace FoodHub.Api.Authentication;

public sealed class GoogleAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string Aud { get; set; } = string.Empty;
}
