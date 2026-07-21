using Delegates.Infrastructure.Entities;
using Delegates.Infrastructure.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.Interfaces
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task<DataTablePaginationResponseDto<TResult>> GetPaginationListFromBodyAsync<TResult>(
            DataTablePaginationRequestDto request,
            Expression<Func<TEntity, TResult>> selector,
            List<Expression<Func<TEntity, bool>>>? criterias = null,
            List<Expression<Func<TEntity, object>>>? includes = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool allowGlobalSearchValue = true,
            bool noTrackingNofilter = false,
            IDictionary<string, string>? columnMap = null,
            CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        IQueryable<TEntity> TableNoTracking { get; }
        IQueryable<TEntity> TableNoTrackingWithNoFilter { get; }
    }
}
