using AutoMapper;
using Delegates.Infrastructure.Entities.Inventory;
using Delegates.Interface.IServices.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.MappingProfile.Inventory
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateUpdateDto, Product>();
        }
    }
}
