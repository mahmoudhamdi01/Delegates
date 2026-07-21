using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Interface.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Repositories
{
    public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
    {
        public string GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.UserType.ToString()),
            new("Code", user.Code)
        };

            if (user.AccountId.HasValue)
                claims.Add(new Claim("AccountId", user.AccountId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTOptions:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JWTOptions:ISSuer"],
                audience: configuration["JWTOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
