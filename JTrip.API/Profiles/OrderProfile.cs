using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JTrip.API.Dtos;
using JTrip.API.Models;

namespace JTrip.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ForMember(des => des.State, opt => opt.MapFrom(src => src.State.ToString()));
        }
    }
}
