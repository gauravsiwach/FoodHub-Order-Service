using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace FoodHub.Api.Authentication;

public sealed class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GoogleJsonWebSignature.Payload> ValidateAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            return await GoogleJsonWebSignature.ValidateAsync(token).ConfigureAwait(false);
        }
        catch
        {
            throw new InvalidOperationException("Invalid Google token");
        }
    }
}
