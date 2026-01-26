namespace Marcum.CCH.Axcess.Infrastructure.Options;

public record AuthorizationTokenOptions
{
    public int TokenRefreshMinutes { get; set; }
    public int TokenExpirationDays { get; set; }
    public required string Directory { get; set; } 
    public required string FileName { get; set; }
}
