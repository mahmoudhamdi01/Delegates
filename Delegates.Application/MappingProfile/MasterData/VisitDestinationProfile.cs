using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Interface.IServices.MasterData.VisitDestination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.MappingProfile.MasterData
{
    public class VisitDestinationProfile : Profile
    {
        public VisitDestinationProfile()
        {
            CreateMap<VisitDestination, VisitDestinationReadDto>();
            CreateMap<VisitDestinationCreateUpdateDto, VisitDestination>();
        }
    }
}
