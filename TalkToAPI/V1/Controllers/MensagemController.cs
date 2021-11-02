using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Contracts;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller")]
    [ApiController]
    [ApiVersion("1.0")]

    public class MensagemController : ControllerBase
    {
        private readonly IMensagemRepository _mensagemRepository;
        public MensagemController(IMensagemRepository mensagemRepository)
        {
            _mensagemRepository = mensagemRepository;
        }

        [Authorize]
        [HttpGet("{usuarioUmId}/{usuarioDoisId}")]
        public ActionResult Obter(string usuarioumid, string usuariodoisid)
        {
            if (usuarioumid == usuariodoisid)
            {
                return UnprocessableEntity();
            }
            return Ok(_mensagemRepository.ObterMensagem(usuarioumid, usuariodoisid));
        }

        [Authorize]
        [HttpPost("")]
        public ActionResult Cadastrar([FromBody]Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mensagemRepository.Cadastrar(mensagem);
                    return Ok(mensagem);
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

            return Ok();
        }
    }
}
