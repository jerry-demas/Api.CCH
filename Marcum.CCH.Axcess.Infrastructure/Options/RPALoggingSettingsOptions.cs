namespace Marcum.CCH.Axcess.Infrastructure.Options;

public record RPALoggingSettingsOptions
{
    public required string OktaSignInUsername { get; set; }
    public required string OktaSignInPassword { get; set; }

}
