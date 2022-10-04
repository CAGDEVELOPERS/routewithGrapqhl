using API.Controller;
using API.Data.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BEFASH.Profiles
{
    public class ClientsProfile : Profile
    {
        public ClientsProfile()
        {
            //Source=> target
            CreateMap<CreateClientDto, Clients>();
            CreateMap<UpdateClientDto, Clients>();


        }
    }
}
