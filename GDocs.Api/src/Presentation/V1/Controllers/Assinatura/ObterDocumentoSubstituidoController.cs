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
    public class ObterDocumentoSubstituidoController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public ObterDocumentoSubstituidoController(
            IAssinaturaAppService assinaturaAppService
            )
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{numeroDocumento}")]
        public async Task<ActionResult<DocumentoFI1548Model>> Get(
            int numeroDocumento,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaAppService.ObterDocumentoFI1548PorNumero(new DocumentoFI1548FilterModel { Numero = numeroDocumento }, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var processo = response.Success;

            return this.Success(processo);
        }
    }
}