using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Exceptions.ErrorModels
{
    public class ValidationError
    {
        public string Field { get; set; } = default!;
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
