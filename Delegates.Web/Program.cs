
using Delegates.Application.Helpers;
using Delegates.Infrastructure.Data.Contexts;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Enums.UserManagement;
using Delegates.Web.ExtensionClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Delegates.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Extension Methods ────────────────────────────────────────────────
            builder.Services.AddPresentationAndSwagger();
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityAndJwt(builder.Configuration);

            // ── Persistence ──────────────────────────────────────────────────────
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // ── Middleware Pipeline ──────────────────────────────────────────────
            app.UseMiddleware<LanguageMiddleware>();
            app.UseMiddleware<CustomExceptionHandler>();
            app.UseMiddleware<JwtCookieToHeaderMiddleware>();

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwaggerWithUI();   // Swagger + UI + root redirect "/"
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();

                var superAdminExists = await db.Users
                    .IgnoreQueryFilters()
                    .AnyAsync(x => x.UserType == UserType.SuperAdmin);

                if (!superAdminExists)
                {
                    var superAdmin = new ApplicationUser
                    {
                        FullName = "Super Admin",
                        PhoneNumber = "01000000000", // غيّره لرقمك الحقيقي قبل النشر
                        Code = "U-00001",
                        UserType = UserType.SuperAdmin,
                        AccountId = null,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    superAdmin.PasswordHash = hasher.HashPassword(superAdmin, "ChangeMe123!"); // غيّرها فورًا بعد أول دخول

                    db.Users.Add(superAdmin);
                    await db.SaveChangesAsync();
                }
            }

            app.Run();
        }
    }
}
