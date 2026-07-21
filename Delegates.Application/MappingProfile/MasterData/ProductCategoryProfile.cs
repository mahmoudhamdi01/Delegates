using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Interface.IServices.MasterData.ProductCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.MappingProfile.MasterData
{
    public class ProductCategoryProfile : Profile
    {
        public ProductCategoryProfile()
        {
            CreateMap<ProductCategory, ProductCategoryReadDto>();
            CreateMap<ProductCategoryCreateUpdateDto, ProductCategory>();
        }
    }
}
