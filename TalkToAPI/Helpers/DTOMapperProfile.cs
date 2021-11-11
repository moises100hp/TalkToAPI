using AutoMapper;
using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models.DTO;

namespace TalkToAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        public DTOMapperProfile()
        {
            CreateMap<AplicationUser, UsuarioDTO>()
                .ForMember(dest=> dest.Nome, orig=> orig.MapFrom(src=> src.FullName));

            CreateMap<AplicationUser, UsuarioDTOSemHyperlink>()
                .ForMember(dest => dest.Nome, orig => orig.MapFrom(src => src.FullName));

            CreateMap<Mensagem, MensagemDTO>();

            //CreateMap<PaginationList<Palavra>, PaginationList<PalavraDTO>>();
        }
    }
}
