using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.Helpers.Constants;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Models.DTO;
using TalkToAPI.V1.Repositories.Contracts;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [EnableCors] //Default Policy

    public class MensagemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMensagemRepository _mensagemRepository;
        public MensagemController(IMensagemRepository mensagemRepository, IMapper mapper)
        {
            _mensagemRepository = mensagemRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("{usuarioumid}/{usuariodoisid}", Name = "MensagemObter")]
        public ActionResult Obter(string usuarioumid, string usuariodoisid, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (usuarioumid == usuariodoisid)
            {
                return UnprocessableEntity();
            }

            var mensagens = _mensagemRepository.ObterMensagens(usuarioumid, usuariodoisid);

            if (mediaType == CustomMediaType.Heteoas)
            {
               

                var listaMgs = _mapper.Map<List<Mensagem>, List<MensagemDTO>>(mensagens);

                var lista = new ListaDTO<MensagemDTO>() { Lista = listaMgs };
                lista.Links.Add(new LinkDTO("_self", Url.Link("MensagemObter", new { usuarioumid = usuarioumid, usuariodoisid = usuariodoisid }), "GET"));

                return Ok(lista);
            }
            else
            {
                return Ok(mensagens);
            }
        }

        [Authorize]
        [HttpPost("", Name = "MensagemCadastrar")]
        public ActionResult Cadastrar([FromBody] Mensagem mensagem, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mensagemRepository.Cadastrar(mensagem);

                    if (mediaType == CustomMediaType.Heteoas)
                    {
                        var mensagemDTO = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
                        mensagemDTO.Links.Add(new LinkDTO("_self", Url.Link("MensagemCadastrar", null), "POST"));
                        mensagemDTO.Links.Add(new LinkDTO("_atualizacaoParcial", Url.Link("MensagemAtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));

                        return Ok(mensagemDTO);
                    }
                    else
                    {
                        return Ok(mensagem);
                    }

                      
                }
                catch (Exception e)
                {

                    return UnprocessableEntity(e);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        [Authorize]
        [HttpPatch("{id}", Name = "MensagemAtualizacaoParcial")]
        public ActionResult AtualizacaoParcial(int id, [FromBody] JsonPatchDocument<Mensagem> jsonPatch, [FromHeader(Name = "Accept")] string mediaType)
        {
            /*
                JSONPath - { "op": "add|remove|replace", "path": "texto", "value": "Substituido" }
             */

            if (jsonPatch == null)
                return BadRequest();

            var mensagem = _mensagemRepository.Obter(id);

            jsonPatch.ApplyTo(mensagem);
            mensagem.Atualizado = DateTime.UtcNow;

            _mensagemRepository.Atualizar(mensagem);

            if (mediaType == CustomMediaType.Heteoas)
            {
                var mensagemDTO = _mapper.Map<Mensagem, MensagemDTO>(mensagem);
                mensagemDTO.Links.Add(new LinkDTO("_self", Url.Link("MensagemAtualizacaoParcial", new { id = mensagem.Id }), "PATCH"));

                return Ok(mensagemDTO);
            }
            else
            {
                return Ok(mensagem);
            }   
        }
    }
}
