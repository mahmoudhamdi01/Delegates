using Delegates.Infrastructure.Data.Contexts;
using Delegates.Infrastructure.Entities;
using Delegates.Infrastructure.Shared.Pagination;
using Delegates.Interface.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.Repositories
{
    public class GenericRepository<TEntity, TKey>(ApplicationDbContext _dbContext)
    : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        protected DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

        public async Task<DataTablePaginationResponseDto<TResult>> GetPaginationListFromBodyAsync<TResult>(
            DataTablePaginationRequestDto request,
            Expression<Func<TEntity, TResult>> selector,
            List<Expression<Func<TEntity, bool>>>? criterias = null,
            List<Expression<Func<TEntity, object>>>? includes = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool allowGlobalSearchValue = true,
            bool noTrackingNofilter = false,
            IDictionary<string, string>? columnMap = null,
            CancellationToken cancellationToken = default)
        {
            var query = noTrackingNofilter ? TableNoTrackingWithNoFilter : TableNoTracking;

            if (criterias != null)
                foreach (var criteria in criterias)
                    query = query.Where(criteria);

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            // العدد الكلي قبل تطبيق الفلاتر/البحث (بعد الـ criterias الإجبارية بس)
            var totalRecords = await query.CountAsync(cancellationToken);

            // فلترة: بحث عام + فلاتر أعمدة (single) + multi-values + date range
            query = query.ApplyDataTableFiltering(request, columnMap, allowGlobalSearchValue);

            // العدد بعد الفلترة
            var filteredRecords = await query.CountAsync(cancellationToken);

            // ترتيب
            if (!string.IsNullOrWhiteSpace(request.SortColumnName))
                query = query.ApplyDataTableSorting(request, columnMap);
            else if (orderBy != null)
                query = orderBy(query);
            else
                query = ApplyDefaultOrder(query);

            // حماية من قيم Start/Length غير منطقية
            var start = Math.Max(request.Start, 0);
            var length = request.Length <= 0 ? 10 : Math.Min(request.Length, 500);

            var data = await query
                .Select(selector)
                .Skip(start)
                .Take(length)
                .ToListAsync(cancellationToken);

            return new DataTablePaginationResponseDto<TResult>
            {
                Draw = request.Draw,
                Data = data,
                RecordsTotal = totalRecords,
                RecordsFiltered = filteredRecords,
                TotalCount = filteredRecords
            };
        }

        private static IQueryable<TEntity> ApplyDefaultOrder(IQueryable<TEntity> query)
        {
            if (typeof(TEntity).GetProperty("CreatedOn") != null)
                return query.OrderByDynamic("CreatedOn", desc: true);
            if (typeof(TEntity).GetProperty("Id") != null)
                return query.OrderByDynamic("Id", desc: true);
            return query;
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
            => await _dbContext.AddAsync(entity, cancellationToken);

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await TableNoTracking.FirstOrDefaultAsync(x => x.Id!.Equals(id), cancellationToken);

        public void Remove(TEntity entity)
            => _dbContext.Remove(entity);

        public void Update(TEntity entity)
            => _dbContext.Update(entity);

        public virtual IQueryable<TEntity> TableNoTracking
            => DbSet.AsNoTracking().AsQueryable();

        public virtual IQueryable<TEntity> TableNoTrackingWithNoFilter
            => DbSet.AsNoTracking().IgnoreQueryFilters().AsQueryable();
    }
}
