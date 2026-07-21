using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class OrderContactLogReadDto
    {
        public string? Notes { get; set; }
        public string? ContactedBy { get; set; }
        public DateTime? ContactedOn { get; set; }
    }
}
