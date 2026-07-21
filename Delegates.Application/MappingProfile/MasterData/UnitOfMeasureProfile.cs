using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Interface.IServices.MasterData.UnitOfMeasure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.MappingProfile.MasterData
{
    public class UnitOfMeasureProfile : Profile
    {
        public UnitOfMeasureProfile()
        {
            CreateMap<UnitOfMeasure, UnitOfMeasureReadDto>();
            CreateMap<UnitOfMeasureCreateUpdateDto, UnitOfMeasure>();
        }
    }
}
