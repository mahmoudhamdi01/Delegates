using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.VisitDestination
{
    public class VisitDestinationCreateUpdateDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;
        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
