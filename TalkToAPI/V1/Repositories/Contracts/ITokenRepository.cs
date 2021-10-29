using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Repositories.Contracts
{
    public interface ITokenRepository
    {
        //C - R- U

        void Cadastrar(Token token);
        Token Obter(String refreshToken);
        void Atualizar(Token token);
    }
}
