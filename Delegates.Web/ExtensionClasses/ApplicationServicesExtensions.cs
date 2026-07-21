using Delegates.Application.MappingProfile.MasterData;
using Delegates.Application.Repositories;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Shared;
using Delegates.Interface.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Delegates.Web.ExtensionClasses
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // AutoMapper - scans the entire assembly of ProductProfile
            services.AddAutoMapper(typeof(WarehouseProfile).Assembly);

            // HttpContext
            services.AddHttpContextAccessor();


            // Core Repositories & Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Localization & Audit
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<EntityAuditHelper>();
            services.AddScoped<IEntityAuditHelper>(sp => sp.GetRequiredService<EntityAuditHelper>());
            services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<EntityAuditHelper>());
            
            return services;
        }
    }
}
