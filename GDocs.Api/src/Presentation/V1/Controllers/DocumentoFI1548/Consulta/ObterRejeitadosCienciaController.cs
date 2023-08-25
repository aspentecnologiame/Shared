using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta
{
    [ApiController]
    [AuthorizeBearer(Roles = "fi1548:consultar")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentoFI1548/consulta/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterRejeitadosCienciaController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ObterRejeitadosCienciaController(IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<SoclicitacaoCienciaAprovadoresModel>> Get(int documentoId, CancellationToken cancellationToken = default)
        {
            var cienciaCancelamento = await _documentoFI1548AppService.ObterAprovadoresCienciaCancelamento(documentoId, cancellationToken);

            if (cienciaCancelamento.IsFailure)
                return this.Failure(cienciaCancelamento.Failure);


            return this.Success(cienciaCancelamento.Success);
        }
    }
}
