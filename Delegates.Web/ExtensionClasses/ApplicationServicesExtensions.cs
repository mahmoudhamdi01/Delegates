using Delegates.Application.MappingProfile.MasterData;
using Delegates.Application.Repositories;
using Delegates.Application.Services.UserManagement;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Shared;
using Delegates.Interface.Interfaces;
using Delegates.Interface.IServices.RegisterDevice;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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

            services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();

            try
            {
                var credentialPath = configuration["Firebase:ServiceAccountKeyPath"];
                if (FirebaseApp.DefaultInstance is null && !string.IsNullOrWhiteSpace(credentialPath) && File.Exists(credentialPath))
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(credentialPath)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase initialization failed: {ex.Message}");
            }

            // Localization & Audit
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<EntityAuditHelper>();
            services.AddScoped<IEntityAuditHelper>(sp => sp.GetRequiredService<EntityAuditHelper>());
            services.AddScoped<ICurrentUserContext>(sp => sp.GetRequiredService<EntityAuditHelper>());
            
            return services;
        }
    }
}
