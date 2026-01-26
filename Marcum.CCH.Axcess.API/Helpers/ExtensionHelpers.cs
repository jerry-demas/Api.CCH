

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Marcum.CCH.Axcess.API.Helpers;

internal static class ExtensionHelpers
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {

        services.AddSwaggerGen(
             o =>
             {
                 o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                 o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                 {
                     Description = "Enter your JWT token in this field",
                     Name = "JWT Authentication",
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.Http,
                     Scheme = JwtBearerDefaults.AuthenticationScheme
                 });


                 o.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                 });
             });

        return services;
    }
}
