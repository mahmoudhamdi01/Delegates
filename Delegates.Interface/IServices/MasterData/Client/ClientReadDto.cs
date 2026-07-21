using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.MasterData.Client
{
    public class ClientReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? SecondaryPhoneNumber { get; set; }
        public string Governorate { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
