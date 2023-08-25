using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
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
    public class ListarDocumentosPendentesAssinaturaOuJaAssinadosPeloUsuarioController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public ListarDocumentosPendentesAssinaturaOuJaAssinadosPeloUsuarioController(
            IAssinaturaAppService assinaturaAppService
        )
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<ProcessoAssinaturaDocumentoModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProcessoAssinaturaDocumentoModel>>> Get(
            CancellationToken cancellationToken = default
        )
        {
            var usuario = this.ObterUsuario();

            var response = await _assinaturaAppService.ListarDocumentosPendentesDeAssinaturaOuJaAssinadoPeloUsuario(usuario.ActiveDirectoryId, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);
        }
    }
}