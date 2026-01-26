using Marcum.CCH.Axcess.Infrastructure.Options;

namespace Marcum.CCH.Axcess.API;

public static class ProgramServiceRegistration
{

    public static IServiceCollection AddProgramServices(this IServiceCollection services, IConfiguration configuration)
    {
       
        services.Configure<ProgramOptions>(configuration.GetSection(nameof(ProgramOptions)));
        services.Configure<CchSettingsOptions>(configuration.GetSection(nameof(CchSettingsOptions)));
        services.Configure<AuthorizationTokenOptions>(configuration.GetSection(nameof(AuthorizationTokenOptions)));
        services.Configure<RPALoggingSettingsOptions>(configuration.GetSection(nameof(RPALoggingSettingsOptions)));
        services.AddHttpClient();
        return services;
    }


}
