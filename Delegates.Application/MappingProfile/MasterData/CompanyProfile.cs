using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Interface.IServices.MasterData.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.MappingProfile.MasterData
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyReadDto>();
            CreateMap<CompanyCreateUpdateDto, Company>();
        }
    }
}
