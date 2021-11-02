using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Models;

namespace TalkToAPI.V1.Repositories.Contracts
{
    public interface IMensagemRepository
    {
        List<Mensagem> ObterMensagem(string usuarioUmId, string usuarioDoisId);

        void Cadastrar(Mensagem mensagem);
        
    }
}
