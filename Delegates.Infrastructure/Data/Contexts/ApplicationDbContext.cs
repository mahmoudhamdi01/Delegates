using Delegates.Infrastructure.Entities.Inventory;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Infrastructure.Entities.Orders;
using Delegates.Infrastructure.Entities.UserManagement;
using Delegates.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserContext _currentUserContext;

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<VisitDestination> VisitDestinations { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderWarehouseTask> OrderWarehouseTasks { get; set; }
        public DbSet<OrderWarehouseTaskItem> OrderWarehouseTaskItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<OrderPostponedCompany> OrderPostponedCompanies { get; set; }
        public DbSet<OrderPostponedCompanyProduct> OrderPostponedCompanyProducts { get; set; }
        public DbSet<OrderContactLog> OrderContactLogs { get; set; }
        public DbSet<Visit> Visits { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserContext currentUserContext)
            : base(options)
        {
            _currentUserContext = currentUserContext;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (!typeof(ISoftDelete).IsAssignableFrom(clrType)) continue;

                // Account مستثنى من فلتر الـ Tenant لأنه هو نفسه الـ Tenant
                var isTenantScoped = typeof(IHasTenant).IsAssignableFrom(clrType) && clrType != typeof(Account);
                var methodName = isTenantScoped ? nameof(SetSoftDeleteAndTenantFilter) : nameof(SetSoftDeleteFilter);

                var method = typeof(ApplicationDbContext)
                    .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.MakeGenericMethod(clrType);

                method?.Invoke(this, new object[] { modelBuilder });
            }
        }

        private void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, ISoftDelete
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => !x.IsDeleted);
        }

        private void SetSoftDeleteAndTenantFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, ISoftDelete, IHasTenant
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x =>
                !x.IsDeleted && (_currentUserContext.IsSuperAdmin || x.AccountId == _currentUserContext.AccountId));
        }
    }
}
