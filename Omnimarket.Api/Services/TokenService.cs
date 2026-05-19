using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Omnimarket.Api.Models;

namespace Omnimarket.Api.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string token, DateTime expiraEmUtc) GerarToken(Usuario usuario)
        {
            var expireMinutes = 60d;
            var expireConfig = _configuration["Jwt:ExpireMinutes"];

            if (!string.IsNullOrWhiteSpace(expireConfig) &&
                double.TryParse(expireConfig, out var parsedExpireMinutes) &&
                parsedExpireMinutes > 0)
            {
                expireMinutes = parsedExpireMinutes;
            }

            var expiraEmUtc = DateTime.UtcNow.AddMinutes(expireMinutes);

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiraEmUtc,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiraEmUtc);
        }
    }
}
