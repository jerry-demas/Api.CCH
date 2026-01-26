
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Marcum.CCH.Axcess.API.Helpers;

    public class TokenProvider(IConfiguration configuration)
    {

        public string CreateToken()
        {
            string secretKey = configuration["Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDesciptor = new SecurityTokenDescriptor { 
                Subject = new System.Security.Claims.ClaimsIdentity([
                    new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, configuration["Jwt:Secret"]!)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!

            };
            

            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(tokenDesciptor);
            
            return token;

        }

    }

