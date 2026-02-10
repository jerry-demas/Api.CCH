
namespace Marcum.CCH.Axcess.Infrastructure.Options;

public record AuthenticationOptions
{
    public required string SigningKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string ExpirationInMinutes { get; set; }


}
