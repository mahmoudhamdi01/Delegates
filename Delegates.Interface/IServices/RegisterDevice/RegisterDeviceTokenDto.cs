using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.RegisterDevice
{
    public class RegisterDeviceTokenDto
    {
        [Required, MaxLength(500)]
        public string Token { get; set; } = default!;
        [MaxLength(20)]
        public string? Platform { get; set; }
    }
}
