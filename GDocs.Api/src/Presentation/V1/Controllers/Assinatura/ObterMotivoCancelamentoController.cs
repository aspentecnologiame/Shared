using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ICE.GDocs.Api.V1.Controllers.Assinatura
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Assinatura/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterMotivoCancelamentoController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ObterMotivoCancelamentoController(IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<ObterMotivoCancelamentoModel>> Get(int documentoId, CancellationToken cancellationToken = default)
        {
            var motivoCancelamento = await _documentoFI1548AppService.ObterMotivoCancelamento(documentoId, cancellationToken);

            if (motivoCancelamento.IsFailure)
                return this.Failure(motivoCancelamento.Failure);


            return this.Success(motivoCancelamento.Success);
        }
    }
}
