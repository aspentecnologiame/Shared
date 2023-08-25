using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using MassTransit;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class CancelarController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;

        public CancelarController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }
        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:cancelar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CancelarOuSolicitarCancelamento")]
        public async Task<IActionResult> CancelarOuSolicitarCancelamento(int idMaterialSaidaNF,string motivo, CancellationToken cancellationToken = default)
        {
            var result = await _saidaMaterialNotaFiscalAppService.CancelarOuSolicitarCancelamento(idMaterialSaidaNF, motivo, cancellationToken);

            if (result.IsFailure)
                return this.Failure(result.Failure);

            return Ok(result.Success);
        }



        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:efetivarCancelamento", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("EfetivarCancelamento")]
        public async Task<IActionResult> EfetivarCancelamento(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken = default)
        {
            var result = await _saidaMaterialNotaFiscalAppService.EfetivarCancelamento(saidaMaterialNotaFiscalModel, cancellationToken);

            if (result.IsFailure)
                return this.Failure(result.Failure);

            return Ok(result.Success);
        }
                
    }
}
