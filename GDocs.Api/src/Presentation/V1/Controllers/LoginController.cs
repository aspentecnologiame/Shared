using ICE.GDocs.Api.Security;
using ICE.GDocs.Application;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class LoginController : ControllerBase
    {
        private readonly SigningConfigurations _signingConfigurations;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly IUsuarioAppService _usuarioAppService;

        public LoginController(
            SigningConfigurations signingConfigurations,
            TokenConfigurations tokenConfigurations,
            IUsuarioAppService usuarioAppService
            )
        {
            _signingConfigurations = signingConfigurations;
            _tokenConfigurations = tokenConfigurations;
            _usuarioAppService = usuarioAppService;
        }

        [Authorize]
        [HttpPost("integrado")]
        [ProducesResponseType(typeof(AccessData), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AccessData>> PostIntegrated(
            CancellationToken cancellationToken = default
        )
            => await Post(new AccessCredentials { GrantType = "integrated" }, cancellationToken);

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(AccessData), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AccessData>> Post(
            [FromBody] AccessCredentials credenciais,
            CancellationToken cancellationToken = default
        )
        {
            switch (credenciais.GrantType)
            {
                case "integrated": return await ObterTokenPorIntegratedAuthentication(cancellationToken);
                case "password": return await ObterTokenPorPasswordAuthentication(credenciais, cancellationToken);
                case "refreshToken": return await ObterTokenPorRefreshTokenAuthentication(credenciais, cancellationToken);
                case "guidSignature": return await ObterTokenPorGuidAssinatura(credenciais, cancellationToken);
                default: return this.Failure(new BusinessException("grantType", "GrantType não encontrado."));
            }
        }

        #region [Metodos Privados]

        private async Task<AccessData> GenerateTokenAsync(UsuarioModel usuario)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, usuario.ActiveDirectoryId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioDeRede),
                new Claim(ClaimTypes.Version, GetType().Assembly.GetName().Version.ToString())
            };



            claims.AddRange(usuario.Perfis.ConvertAll(perfil => new Claim(CustomClaimsTypes.ProfileId, perfil.Id.ToString())));
            claims.AddRange(usuario.Permissoes.ConvertAll(permissao => new Claim(ClaimTypes.Role, permissao.Chave)));

            var identity = new ClaimsIdentity(
                new GenericIdentity(usuario.Nome, "Login"),
                claims
            );

            var dataCriacao = DateTime.Now;
            var dataExpiracao = dataCriacao + _tokenConfigurations.Validade;

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            var resultado = new AccessData
            {
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString().Replace("-", string.Empty),
                ExpiresInSeconds = Convert.ToInt64(_tokenConfigurations.Validade.TotalSeconds),
                UserData = new UserData
                {
                    FullName = usuario.Nome,
                    UserName = usuario.UsuarioDeRede,
                    Email = usuario.Email,
                    Id = usuario.ActiveDirectoryId,
                    Menu = usuario.Permissoes
                    .Where(permissao => permissao.TipoFuncionalidadeId == TipoFuncionalidade.Menu.ToInt32())
                    .Select(permissao =>
                        new FuncionalidadeModel
                        {
                            Id = permissao.Id,
                            Icone = permissao.Icone,
                            Chave = permissao.Chave,
                            Texto = permissao.Texto,
                            IdPai = permissao.IdPai,
                            Rota = permissao.Rota,
                            Ordem = permissao.Ordem
                        })
                    .AsList()
                }
            };

            var refreshTokenModel = new RefreshTokenModel
            {
                RefreshToken = resultado.RefreshToken,
                ActiveDirectoryId = usuario.ActiveDirectoryId,
                FullName = usuario.Nome,
                UserName = usuario.UsuarioDeRede,
                Email = usuario.Email
            };

            await _usuarioAppService.InserirRefreshToken(refreshTokenModel, _tokenConfigurations.ValidadeRefresh);

            return resultado;
        }

        private async Task<ObjectResult> ObterTokenPorIntegratedAuthentication(CancellationToken cancellationToken)
        {
            var usuarioAutenciacacao = await _usuarioAppService.AutenticarUsuarioPorNomeUsuario(User.Identity.Name, cancellationToken);

            if (usuarioAutenciacacao.IsFailure)
                return this.Failure(usuarioAutenciacacao.Failure);

            return this.Success(await GenerateTokenAsync(usuarioAutenciacacao.Success));
        }

        private async Task<ObjectResult> ObterTokenPorPasswordAuthentication(AccessCredentials credenciais, CancellationToken cancellationToken)
        {
            var usuarioAutenciacacao = await _usuarioAppService.AutenticarUsuario(credenciais.UserName, credenciais.Password, cancellationToken);

            if (usuarioAutenciacacao.IsFailure)
                return this.Failure(usuarioAutenciacacao.Failure);

            return this.Success(await GenerateTokenAsync(usuarioAutenciacacao.Success));
        }

        private async Task<ObjectResult> ObterTokenPorRefreshTokenAuthentication(AccessCredentials credenciais, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(credenciais.RefreshToken))
                return this.Failure(new BusinessException("impossivel-renovacao-token", "Refresh Token não encontrado."));

            var tokenArmazenado = await _usuarioAppService.ObterRefreshToken(credenciais.RefreshToken);

            if (tokenArmazenado.IsFailure || tokenArmazenado.Success == default)
                return this.Failure(new BusinessException("impossivel-renovacao-token", "Não foi possível renovar as credenciais!"));

            var refreshTokenBase = tokenArmazenado.Success;

            var credenciaisValidas = (refreshTokenBase != null && credenciais.UserName == refreshTokenBase.UserName && credenciais.RefreshToken == refreshTokenBase.RefreshToken);

            if (!credenciaisValidas)
                return this.Failure(new BusinessException("impossivel-renovacao-token", "Não foi possível renovar as credenciais!"));

            await _usuarioAppService.RemoverRefreshToken(credenciais.RefreshToken);

            var usuarioAutenciacacao = await _usuarioAppService.ObterUsuarioPorActiveDirectoryId(refreshTokenBase.ActiveDirectoryId, cancellationToken);

            if (usuarioAutenciacacao.IsFailure)
                return this.Failure(usuarioAutenciacacao.Failure);

            var usuario = usuarioAutenciacacao.Success;

            usuario.Nome = refreshTokenBase.FullName;
            usuario.UsuarioDeRede = refreshTokenBase.UserName;
            usuario.Email = refreshTokenBase.Email;

            return this.Success(await GenerateTokenAsync(usuario));
        }

        private async Task<ObjectResult> ObterTokenPorGuidAssinatura(AccessCredentials credentials, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(credentials.GuidSignature))
                return this.Failure(new BusinessException("impossivel-autenticar-guid", "A chave de acesso contida na URL é inválida! Será necessário fazer o login para acessar o sistema."));

            var usuarioAdArmazenado = await _usuarioAppService.ObterGuidUsuarioAdPorChaveDeAcessoEmail(credentials.GuidSignature);

            if (usuarioAdArmazenado.IsFailure || usuarioAdArmazenado.Success == Guid.Empty)
                return this.Failure(new BusinessException("impossivel-autenticar-guid", "Essa URL de acesso expirou! Será necessário fazer o login para acessar o sistema."));

            await _usuarioAppService.RemoverChaveDeAcessoEmail(credentials.GuidSignature);

            var listaUsuarioAd = await _usuarioAppService.ListarUsuariosActiveDirectory(usuarioAdArmazenado.Success, cancellationToken);

            if (listaUsuarioAd.IsFailure)
                return this.Failure(listaUsuarioAd.Failure);

            if (listaUsuarioAd.Success.Empty())
                return this.Failure(new BusinessException("impossivel-autenticar-guid", "Essa URL não pertence ao usuário que está tentando acessá-la! Será necessário fazer o login para acessar o sistema."));

            var usuarioAd = listaUsuarioAd.Success.First();

            var usuarioAutenciacacao = await _usuarioAppService.ObterUsuarioPorActiveDirectoryId(usuarioAdArmazenado.Success, cancellationToken);

            if (usuarioAutenciacacao.IsFailure)
                return this.Failure(usuarioAutenciacacao.Failure);

            var usuario = usuarioAutenciacacao.Success;

            usuario.Nome = usuarioAd.Nome;
            usuario.UsuarioDeRede = usuarioAd.UsuarioDeRede;
            usuario.Email = usuarioAd.Email;

            return this.Success(await GenerateTokenAsync(usuario));
        }

        #endregion
    }
}