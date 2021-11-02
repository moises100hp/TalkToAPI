using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller")]
    [ApiController]
    [ApiVersion("1.0")]

    public class MensagemController : ControllerBase
    {
       public ActionResult ObterMensagens()
        {
            return Ok();
        }

        public ActionResult Cadastrar()
        {
            return Ok();
        }
    }
}
