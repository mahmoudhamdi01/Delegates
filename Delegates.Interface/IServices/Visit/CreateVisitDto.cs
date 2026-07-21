using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.Visit
{
    public class CreateVisitDto
    {
        [Required]
        public int VisitDestinationId { get; set; }
        [Required]
        public int DelegateId { get; set; }
        [Required, MaxLength(300)]
        public string Address { get; set; } = default!;
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
