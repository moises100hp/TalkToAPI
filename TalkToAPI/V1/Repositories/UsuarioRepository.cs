using Microsoft.AspNetCore.Identity;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkToAPI.V1.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<AplicationUser> _userManager;

        public UsuarioRepository(UserManager<AplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public AplicationUser Obter(string email, string senha)
        {
            var usuario = _userManager.FindByEmailAsync(email).Result;

            if (_userManager.CheckPasswordAsync(usuario, senha).Result)
                return usuario;
            else
                throw new Exception("Usuário não localizado!");
                /*
                 * Domain Notification
                 */
        }

        public void Cadastrar(AplicationUser usuario, string senha)
        {
            var result = _userManager.CreateAsync(usuario, senha).Result;

            if(!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var erro in result.Errors)
                {
                    sb.Append(erro.Description);
                }

                throw new Exception($"Usuário não cadastrado! {sb.ToString()}");
            }
        }

        public AplicationUser Obter(string id)
        {
            return _userManager.FindByEmailAsync(id).Result;
        }
    }
}
