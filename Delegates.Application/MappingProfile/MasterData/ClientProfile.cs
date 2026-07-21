using AutoMapper;
using Delegates.Infrastructure.Entities.MasterData;
using Delegates.Interface.IServices.MasterData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Application.MappingProfile.MasterData
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientReadDto>();
            CreateMap<ClientCreateUpdateDto, Client>();
        }
    }
}
