using System.Text;
using Marcum.CCH.Axcess.API;
using Marcum.CCH.Axcess.API.Controllers;
using Marcum.CCH.Axcess.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using CBIZ.SharedPackages;
using Cbiz.SharedPackages;
using Marcum.CCH.Axcess.Infrastructure.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();
builder.AddAzureKeyVaultAsConfig();
builder.Services.AddProgramServices(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<AuthenticationOptions>>((jwtOptions, authOptions) =>
    {
        jwtOptions.RequireHttpsMetadata = false;

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authOptions.Value.SigningKey)
            ),
            ValidIssuer = authOptions.Value.Issuer,
            ValidAudience = authOptions.Value.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });



builder.Services.AddSingleton<TokenProvider>();
builder.Configuration.AddUserSecrets<TaxReturnsController>();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddNLog("NLog.config");
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
