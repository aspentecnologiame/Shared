using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI347.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/consulta/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterRejeitadosCienciaController : ControllerBase
    {
        private readonly ISolicitacaoCienciaAppService _solicitacaoCienciaAppService;

        public ObterRejeitadosCienciaController(ISolicitacaoCienciaAppService solicitacaoCienciaAppService)
        {
            _solicitacaoCienciaAppService = solicitacaoCienciaAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [HttpGet("")]
        [AuthorizeBearer(Roles = "fi347:consultar")]
        public async Task<ActionResult<SoclicitacaoCienciaAprovadoresModel>> Get(int idSaidaMaterial, CancellationToken cancellationToken = default)
        {
            var aprovadoresCienciaCancelamento = await _solicitacaoCienciaAppService.ObterAprovadoresCienciaCancelamento(idSaidaMaterial, cancellationToken);

            if (aprovadoresCienciaCancelamento.IsFailure)
                return this.Failure(aprovadoresCienciaCancelamento.Failure);


            return this.Success(aprovadoresCienciaCancelamento.Success);
        }
    }
}
