using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Repositories.Contracts
{
    public interface IUsuarioRepository
    {
        void Cadastrar(AplicationUser usuario, string senha);
        AplicationUser Obter(string email, string senha);
        AplicationUser Obter(string id);

    }
}
