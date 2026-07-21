using Delegates.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Entities
{
    public class BaseEntity<TKey> : ISoftDelete, IHasTenant
    {
        public TKey Id { get; set; }
        public int? AccountId { get; set; }
        public string? CreatedBy { get; set; } // User Id
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; } // User Id
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; } // Soft Delete
    }
}
