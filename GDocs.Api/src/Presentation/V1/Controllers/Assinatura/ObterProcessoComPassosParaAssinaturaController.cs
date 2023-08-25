using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
    public class ObterProcessoComPassosParaAssinaturaController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public ObterProcessoComPassosParaAssinaturaController(
            IAssinaturaAppService assinaturaAppService
            )
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{processoId}")]
        public async Task<ActionResult<AssinaturaModel>> Get(
            long processoId,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaAppService.ObterProcessoComPassosParaAssinatura(processoId, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var processo = response.Success;

            if (!processo.Passos.Itens.Any())
                return NotFound();

            return this.Success(processo);
        }
    }
}