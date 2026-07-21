using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Infrastructure.Exceptions.ErrorModels
{
    public class ErrorToReturn
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = default!;
        public List<string> Errors { get; set; }
    }
}
