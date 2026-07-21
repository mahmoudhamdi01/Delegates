using Delegates.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.Interfaces
{
    public interface IEntityAuditHelper
    {
        void SetCreated<TKey>(BaseEntity<TKey> entity);
        void SetUpdated<TKey>(BaseEntity<TKey> entity);
        void SetSoftDeleted<TKey>(BaseEntity<TKey> entity);
        string? GetCurrentUserId();
    }
}
