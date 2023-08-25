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
    [Route("v{version:apiVersion}/DocumentoFI1548/ObterStatusPagamentoPorId")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterStatusPagamentoPorIdController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ObterStatusPagamentoPorIdController(
            IAuthorizationService authService,
            IDocumentoFI1548AppService documentoFI1548AppService
            )
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<DocumentoFI1548Model>> Post(
            int id,
            CancellationToken cancellationToken = default
        )
        {

            var response = await _documentoFI1548AppService.ObterPorId(id, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var StatusPagamento = response.Success;

            if (StatusPagamento == null)
                return NotFound();

            return this.Success(StatusPagamento);
        }
    }
}
