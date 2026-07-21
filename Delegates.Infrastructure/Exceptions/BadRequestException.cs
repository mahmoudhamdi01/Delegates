using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Exceptions
{
    public class BadRequestException(List<string> Errors) : Exception("Validation Failed")
    {
        public List<string> Errors { get; } = Errors;
    }
}
