using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using TalkToAPI.V1.Models;
using System.Security.Claims;
using System.Text;
using TalkToAPI.V1.Repositories.Contracts;
using TalkToAPI.V1.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using TalkToAPI.Helpers.Constants;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly SignInManager<AplicationUser> _signInManager;
        private readonly UserManager<AplicationUser> _userManager;

        public UsuarioController(IMapper mapper, IUsuarioRepository usuarioRepository, ITokenRepository tokenRepository, SignInManager<AplicationUser> signInManager, UserManager<AplicationUser> userManager)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("", Name = "UsuarioObterTodos")]
        public ActionResult ObterTodos([FromHeader(Name = "Accept")] string mediaType)
        {
            var usuarioAppUser = _userManager.Users.ToList();

            if(mediaType == CustomMediaType.Heteoas)
            {
                var listaUsuarioDTO = _mapper.Map<List<AplicationUser>, List<UsuarioDTO>>(usuarioAppUser);

                foreach (var usuarioDTO in listaUsuarioDTO)
                {
                    usuarioDTO.Links.Add(new LinkDTO("_self", Url.Link("UsuarioObter", new { id = usuarioDTO.Id }), "GET"));
                }

                var lista = new ListaDTO<UsuarioDTO>() { Lista = listaUsuarioDTO };
                lista.Links.Add(new LinkDTO("_self", Url.Link("UsuarioObterTodos", null), "GET"));

                return Ok(lista);
            }
            else
            {
                var usuarioResult = _mapper.Map<List<AplicationUser>, List<UsuarioDTOSemHyperlink>>(usuarioAppUser);
                return Ok(usuarioResult);

                //TODO - AutoMapper -> Converter para objeto sem HyperLink.
                //return Ok(usuarioAppUser);
            }
        }

        [HttpGet("{id}", Name = "UsuarioObter")]
        public ActionResult ObterUsuario(string id, [FromHeader(Name = "Accept")] string mediaType)
        {
            var usuario = _userManager.FindByIdAsync(id).Result;

            if (usuario == null)
                return NotFound();

            if(mediaType == CustomMediaType.Heteoas)
            {
                var usuarioDTOdb = _mapper.Map<AplicationUser, UsuarioDTO>(usuario);
                usuarioDTOdb.Links.Add(new LinkDTO("_self", Url.Link("UsuarioObter", new { id = usuario.Id }), "GET"));
                usuarioDTOdb.Links.Add(new LinkDTO("_atualizar", Url.Link("UsuarioAtualizar", new { id = usuario.Id }), "PUT"));

                return Ok(usuarioDTOdb);
            }
            else
            {
                var usuarioResult = _mapper.Map<AplicationUser, UsuarioDTOSemHyperlink>(usuario);
                return Ok(usuarioResult);
            }
        }

        [HttpPost("", Name = "UsuarioCadastrar")]
        public ActionResult Cadastrar([FromBody] UsuarioDTO usuarioDTO, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (ModelState.IsValid)
            {
                AplicationUser usuario = new AplicationUser();

                usuario.FullName = usuarioDTO.Nome;
                usuario.UserName = usuarioDTO.Email;
                usuario.Email = usuarioDTO.Email;

                var resultado = _userManager.CreateAsync(usuario, usuarioDTO.Senha).Result;

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();

                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);
                    }
                    return UnprocessableEntity(erros);
                }
                else
                {
                    if(mediaType == CustomMediaType.Heteoas)
                    {
                        var usuarioDTOdb = _mapper.Map<AplicationUser, UsuarioDTO>(usuario);
                        usuarioDTOdb.Links.Add(new LinkDTO("_self", Url.Link("UsuarioCadastrar", new { id = usuario.Id }), "POST"));
                        usuarioDTOdb.Links.Add(new LinkDTO("_obter", Url.Link("UsuarioObter", new { id = usuario.Id }), "GET"));
                        usuarioDTOdb.Links.Add(new LinkDTO("_atualizar", Url.Link("UsuarioAtualizar", new { id = usuario.Id }), "PUT"));

                        return Ok(usuarioDTOdb);
                    }
                    else
                    {
                        var usuarioResult = _mapper.Map<AplicationUser, UsuarioDTOSemHyperlink>(usuario);
                        return Ok(usuarioResult);
                    }
                }
            }
            else
                return UnprocessableEntity(ModelState);
        }

        [Authorize]
        [HttpPut("{id}", Name = "UsuarioAtualizar")]
        public ActionResult Atualizar(string id, [FromBody] UsuarioDTO usuarioDTO, [FromHeader(Name = "Accept")] string mediaType)
        {
            AplicationUser usuario = _userManager.GetUserAsync(HttpContext.User).Result;

            //TODO - Adicionar Filtro de validação
            if (usuario.Id != id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                //TODO - Refatorar para AutoMapper

                usuario.FullName = usuarioDTO.Nome;
                usuario.UserName = usuarioDTO.Email;
                usuario.Email = usuarioDTO.Email;
                usuario.Slogan = usuario.Slogan;

                //TODO - Remover no Identity critérios da senha
                var resultado = _userManager.UpdateAsync(usuario).Result;
                _userManager.RemovePasswordAsync(usuario);
                _userManager.AddPasswordAsync(usuario, usuarioDTO.Senha);

                if (!resultado.Succeeded)
                {
                    List<string> erros = new List<string>();

                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);
                    }
                    return UnprocessableEntity(erros);
                }
                else
                {
                    if(mediaType == CustomMediaType.Heteoas)
                    {
                        var usuarioDTOdb = _mapper.Map<AplicationUser, UsuarioDTO>(usuario);
                        usuarioDTOdb.Links.Add(new LinkDTO("_self", Url.Link("UsuarioAtualizar", new { id = usuario.Id }), "PUT"));
                        usuarioDTOdb.Links.Add(new LinkDTO("_obter", Url.Link("UsuarioObter", new { id = usuario.Id }), "GET"));

                        return Ok(usuarioDTOdb);
                    }
                    else
                    {
                        var usuarioResult = _mapper.Map<AplicationUser, UsuarioDTOSemHyperlink>(usuario);
                        return Ok(usuarioResult);
                    }
                }
            }
            else
                return UnprocessableEntity(ModelState);
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UsuarioDTO usuarioDTO)
        {
            ModelState.Remove("Nome");
            ModelState.Remove("ConfirmacaoSenha");

            if (ModelState.IsValid)
            {
                AplicationUser usuario = _usuarioRepository.Obter(usuarioDTO.Email, usuarioDTO.Senha);

                if (usuario != null)
                {
                    //Login no Identity
                    //Será adaptado para JWT

                    // _signInManager.SignInAsync(usuario, false);

                    //retorna Token (JWT)
                    return GerarToken(usuario);
                }
                else
                    return NotFound("Usuário não localizado");
            }
            else
                return UnprocessableEntity(ModelState);
        }


        [HttpPost("renovar")]
        public ActionResult Renovar([FromBody] TokenDTO tokenDTO)
        {
            var refreshTokenDB = _tokenRepository.Obter(tokenDTO.RefreshToken);

            if (refreshTokenDB == null)
                return NotFound();

            //Refresh token usado (antigo) e atualizar - desativar refreshtoken
            refreshTokenDB.Atualizado = DateTime.Now;
            refreshTokenDB.utilizado = true;
            _tokenRepository.Atualizar(refreshTokenDB);


            //Gerar novo token com refresh e salvar

            var usuario = _usuarioRepository.Obter(refreshTokenDB.Usuario.Id);

            //retorna Token (JWT)
            return GerarToken(usuario);
        }


        private TokenDTO BuildToken(AplicationUser usuario)
        {
            var claims = new[]
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, usuario.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave-api-jwt-minhas-tarefas")); //Recomendado -> appSettings.json
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: exp,
                    signingCredentials: sign
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();
            var exprefreshToken = DateTime.UtcNow.AddHours(2);

            var tokenDTO = new TokenDTO { Token = tokenString, Expiration = exp, RefreshToken = refreshToken, ExpirationRefreshToken = exprefreshToken };


            return tokenDTO;
        }

        private ActionResult GerarToken(AplicationUser usuario)
        {
            var token = BuildToken(usuario);

            //Salvar token no banco
            var tokenModel = new Token()
            {
                ExpirationToken = token.Expiration,
                RefreshToken = token.RefreshToken,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                Usuario = usuario,
                Criado = DateTime.Now,
                utilizado = false
            };

            _tokenRepository.Cadastrar(tokenModel);

            return Ok(token);
        }
    }
}
