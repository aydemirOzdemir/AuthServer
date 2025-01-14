﻿using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Mappings
{
    internal class DtoMapper:Profile
    {
        public DtoMapper()
        {
            CreateMap<ProductDto, ProductDto>().ReverseMap();
            CreateMap<UserAppDto, UserApp>().ReverseMap();
        }
    }
}
