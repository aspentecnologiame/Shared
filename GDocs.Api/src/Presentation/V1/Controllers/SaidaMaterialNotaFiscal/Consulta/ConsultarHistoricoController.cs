using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Consulta
{
    [ApiController]
    [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/saidamaterialnotafiscal/consulta/consultarHistorico")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultarHistoricoController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;

        public ConsultarHistoricoController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [HttpGet("")]
        public async Task<ActionResult<SaidaMaterialNotaFiscalHistoricoResponseModel>> Get(int idSaidaMaterialNotaFiscal,
            CancellationToken cancellationToken = default)
        {

            var historicoMaterialNf = await _saidaMaterialNotaFiscalAppService.ObterHistoricoMaterialNf(idSaidaMaterialNotaFiscal, cancellationToken);
            if (historicoMaterialNf.IsFailure)
                return this.Failure(historicoMaterialNf.Failure);


            return historicoMaterialNf.Success;

        }
    }
}
