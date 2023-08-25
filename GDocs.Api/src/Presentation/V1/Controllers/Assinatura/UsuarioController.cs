using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.Assinatura
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Assinatura/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class UsuarioController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public UsuarioController(IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ListarUsuariosActiveDirectory/{nome}"), HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioActiveDirectory>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(
            [FromRoute] string nome,
            CancellationToken cancellationToken = default
        )
        {
            var users = await _assinaturaAppService.ListarUsuariosActiveDirectory(nome, cancellationToken);

            if (users.IsFailure)
                return this.Failure(users.Failure);

            return this.Success(users.Success);
        }
    }
}