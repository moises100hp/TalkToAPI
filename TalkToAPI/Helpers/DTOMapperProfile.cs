﻿using AutoMapper;
using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
           // CreateMap<Palavra, PalavraDTO>();
            //CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();
        }
    }
}