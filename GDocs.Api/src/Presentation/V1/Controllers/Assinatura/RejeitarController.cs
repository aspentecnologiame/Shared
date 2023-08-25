using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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
    public class RejeitarController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public RejeitarController(IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<AssinaturaModel>> Post(AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemRejeitarModel, CancellationToken cancellationToken = default)
        {
            var rejeitar = await _assinaturaAppService.Rejeitar(this.ObterUsuario().ActiveDirectoryId, assinaturaPassoItemRejeitarModel, cancellationToken);
            if (rejeitar.IsFailure)
                return this.Failure(rejeitar.Failure);

            return this.Success(rejeitar.Success);
        }
    }
}