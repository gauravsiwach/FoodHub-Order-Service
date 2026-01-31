using Google.Apis.Auth;

namespace FoodHub.Api.Authentication;

public interface IGoogleTokenValidator
{
    Task<GoogleJsonWebSignature.Payload> ValidateAsync(string token, CancellationToken cancellationToken);
}
