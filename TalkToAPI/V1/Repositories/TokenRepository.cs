using TalkToAPI.Database;
using TalkToAPI.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.V1.Repositories.Contracts;

namespace TalkToAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly TalkToContext _banco;

        public TokenRepository(TalkToContext banco)
        {
            _banco = banco;
        }

        public Token Obter(string refreshToken)
        {
            return _banco.Tokens.FirstOrDefault(a => a.RefreshToken == refreshToken && a.utilizado == false);
        }

        public void Cadastrar(Token token)
        {
            _banco.Tokens.Add(token);
            _banco.SaveChanges();
        }

        public void Atualizar(Token token)
        {
            _banco.Update(token);
            _banco.SaveChanges();
        }
    }
}
