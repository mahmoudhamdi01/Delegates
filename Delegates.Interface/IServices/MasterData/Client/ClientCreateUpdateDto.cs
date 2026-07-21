using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Client
{
    public class ClientCreateUpdateDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = default!;
        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = default!;
        [MaxLength(20)]
        public string? SecondaryPhoneNumber { get; set; }
        [Required, MaxLength(100)]
        public string Governorate { get; set; } = default!;
        [Required, MaxLength(300)]
        public string Address { get; set; } = default!;
    }
}
