using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkToAPI.Database;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Contracts;

namespace TalkToAPI.V1.Repositories
{
    public class MensagemRepository : IMensagemRepository
    {
        private readonly TalkToContext _banco;

        public MensagemRepository(TalkToContext banco)
        {
            _banco = banco;
        }


        public Mensagem Obter(int id)
        {
            return _banco.Mensagem.Find(id);
        }

        public List<Mensagem> ObterMensagens(string usuarioumid, string usuariodoisid)
        {
            return _banco.Mensagem.Where(a => (a.DeId == usuarioumid || a.DeId == usuariodoisid) && (a.ParaId == usuarioumid || a.ParaId == usuariodoisid)).ToList();
        }

        public void Cadastrar(Mensagem mensagem)
        {
            _banco.Mensagem.Add(mensagem);
            _banco.SaveChanges();
        }

        public void Atualizar(Mensagem mensagem)
        {
            _banco.Mensagem.Update(mensagem);
            _banco.SaveChanges();
        }
    }
}
