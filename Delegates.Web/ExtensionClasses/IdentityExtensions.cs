using Delegates.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Delegates.Web.ExtensionClasses
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityAndJwt(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWTOptions:ISSuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["JWTOptions:Audience"],

                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWTOptions:SecretKey"]!))
                };
            });

            return services;
        }
    }
}
