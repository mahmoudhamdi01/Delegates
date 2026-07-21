using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Order
{
    public class AddOrderContactLogDto
    {
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
