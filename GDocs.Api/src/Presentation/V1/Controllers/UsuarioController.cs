using ICE.GDocs.Application;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
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
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioAppService _usuarioAppService;

        public UsuarioController(IUsuarioAppService usuarioAppService)
        {
            _usuarioAppService = usuarioAppService;
        }

        [Route("ListarUsuarios/{idPerfil}/{nome?}"), HttpGet]
        [ApiExplorerSettings(GroupName = "Usuario")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UsuarioModel>>> ListarUsuarios(
            [FromRoute] int idPerfil,
            [FromRoute] string nome = "",
            CancellationToken cancellationToken = default
        )
        {
            var users = await _usuarioAppService.ListarUsuarios(idPerfil, nome, cancellationToken);
            return this.Success(users.Success);
        }


        [Route("ListarUsuariosActiveDirectory/{nome}"), HttpGet]
        [ApiExplorerSettings(GroupName = "Usuario")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioActiveDirectory>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(
            [FromRoute] string nome,
            CancellationToken cancellationToken = default
        )
        {
            var users = await _usuarioAppService.ListarUsuariosActiveDirectory(nome, cancellationToken);

            if (users.IsFailure)
                return this.Failure(users.Failure);

            return this.Success(users.Success);
        }

        [Route("InserirUsuario"), HttpPost]
        [ApiExplorerSettings(GroupName = "Usuario")]
        [ProducesResponseType(typeof(UsuarioModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UsuarioModel>> InserirUsuario(
            UsuarioModel usuarioModel,
            CancellationToken cancellationToken = default
        )
        {
            var users = await _usuarioAppService.ListarUsuariosActiveDirectory(usuarioModel.Nome, cancellationToken);
            if (users.IsSuccess)
            {
                var usuario = users.Success.AsList();
                usuarioModel.Email = usuario[0].Email;
            }
            var resposta = await _usuarioAppService.InserirUsuario(usuarioModel, cancellationToken);

            if (resposta.IsFailure)
                return this.Failure(resposta.Failure);

            return this.Success(resposta.Success);
        }

        [Route("AlterarUsuario"), HttpPost]
        [ApiExplorerSettings(GroupName = "Usuario")]
        [ProducesResponseType(typeof(UsuarioModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<UsuarioModel>> AlterarUsuario(
            UsuarioModel usuarioModel,
            CancellationToken cancellationToken = default
        )
        {
            var resposta = await _usuarioAppService.AlterarUsuario(usuarioModel, cancellationToken);

            if (resposta.IsFailure)
                return this.Failure(resposta.Failure);

            return this.Success(resposta.Success);
        }
    }
}