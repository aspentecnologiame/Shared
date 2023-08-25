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
    public class ListarAssinantesProcessoController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public ListarAssinantesProcessoController(
            IAssinaturaAppService assinaturaAppService
            )
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{processoId}/{todosStatus?}")]
        public async Task<ActionResult<AssinaturaPassoAssinanteRepresentanteModel>> Get(
            long processoId,
            bool todosStatus = false,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaAppService.ListarAssinantesProcesso(processoId, todosStatus, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var assinanteList = response.Success;

            if (assinanteList == null)
                return NotFound();

            return this.Success(assinanteList);
        }
    }
}