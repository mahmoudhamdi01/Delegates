using Delegates.Interface.IServices.PostponeOrderCompany;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Interface.IServices.PostponeOrder
{
    public class PostponeOrderDto
    {
        [Range(0.01, double.MaxValue)]
        public decimal DepositAmount { get; set; }
        [Required, MaxLength(50)]
        public string DepositPaymentMethod { get; set; } = default!;
        [MinLength(1)]
        public List<PostponeOrderCompanyDto> Companies { get; set; } = new();
    }
}
