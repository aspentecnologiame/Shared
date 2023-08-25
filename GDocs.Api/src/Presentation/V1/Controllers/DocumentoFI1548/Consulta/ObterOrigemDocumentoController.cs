using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI1548/ObterOrigemDocumentoPorId")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterOrigemDocumentoPorIdController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ObterOrigemDocumentoPorIdController(
            IDocumentoFI1548AppService documentoFI1548AppService
            )
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<ProcessoAssinaturaDocumentoOrigemModel>> Post(
            int documentoId,
            CancellationToken cancellationToken = default
        )
        {

            var responseOrigem = await _documentoFI1548AppService.ObterOrigemDocumento(documentoId, cancellationToken);

            if (responseOrigem.IsFailure)
                return this.Failure(responseOrigem.Failure);

            var OrigemDocumento = responseOrigem.Success;

            if (OrigemDocumento == null)
                return NotFound();

            return this.Success(OrigemDocumento);
        }
    }
}
