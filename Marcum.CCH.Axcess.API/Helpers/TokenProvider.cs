
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Marcum.CCH.Axcess.Domain.Models;
using Marcum.CCH.Axcess.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium;

namespace Marcum.CCH.Axcess.API.Helpers;

public class TokenProvider
{
    private readonly AuthenticationOptions _authenticationOptions;

    public TokenProvider(IConfiguration configuration, IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }

    public OAuthTicket CreateToken()
    {
        string signingKey = _authenticationOptions.SigningKey;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDesciptor = new SecurityTokenDescriptor { 
            Subject = new System.Security.Claims.ClaimsIdentity([
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, _authenticationOptions.SigningKey)
            ]),
            IssuedAt = DateTime.Now,
            Expires = DateTime.Now.AddMinutes(int.TryParse(_authenticationOptions.ExpirationInMinutes, out int value) ? value : 60),
            SigningCredentials = credentials,
            Issuer = _authenticationOptions.Issuer,
            Audience = _authenticationOptions.Audience

        };
            

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDesciptor);
        return new OAuthTicket(token, string.Empty, tokenDesciptor.IssuedAt, tokenDesciptor.Expires);

    }

}

