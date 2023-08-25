using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI347.Consulta
{
    [ApiController]
    [AuthorizeBearer(Roles = "fi347:consultar")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentoFI347/consulta/consultarHistorico")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultarHistoricoController : ControllerBase
    {
        private readonly ISolicitacaoSaidaMaterialAcaoAppService _solicitacaoSaidaMaterialAcaoAppService;

        public ConsultarHistoricoController(ISolicitacaoSaidaMaterialAcaoAppService solicitacaoSaidaMaterialAcaoAppService)
        {
            _solicitacaoSaidaMaterialAcaoAppService = solicitacaoSaidaMaterialAcaoAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [HttpGet("")]
        public async Task<ActionResult<HistoricoResponseModel>> Get(int idSaidaMaterial,
            CancellationToken cancellationToken = default)
        {
            var historicoMaterialSemNF = await _solicitacaoSaidaMaterialAcaoAppService.ObterHistoricoMaterial(idSaidaMaterial,cancellationToken);

            if (historicoMaterialSemNF.IsFailure)
                return this.Failure(historicoMaterialSemNF.Failure);

            return historicoMaterialSemNF.Success;
        }
    }
}
