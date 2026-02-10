using Marcum.CCH.Axcess.API.Controllers;
using Marcum.CCH.Axcess.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Marcum.CCH.Axcess.API;

public static class ProgramServiceRegistration
{

    public static IServiceCollection AddProgramServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<ProgramOptions>(configuration.GetSection(nameof(ProgramOptions)));
        services.Configure<CchSettingsOptions>(configuration.GetSection(nameof(CchSettingsOptions)));
        services.Configure<AuthorizationTokenOptions>(configuration.GetSection(nameof(AuthorizationTokenOptions)));
        services.Configure<AuthenticationOptions>(configuration.GetSection(nameof(AuthenticationOptions)));       
        services.AddHttpClient();
        return services;
    }


}
