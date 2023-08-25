using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Ciencia
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/ciencia/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class RegistroCienciaNfController : ControllerBase
    {
        private readonly ISolicitacaoCienciaNotaFiscalAppService _solicitacaoCienciaNotaFiscalAppService;
        public RegistroCienciaNfController(ISolicitacaoCienciaNotaFiscalAppService solicitacaoCienciaNotaFiscalAppService)
        {
            _solicitacaoCienciaNotaFiscalAppService = solicitacaoCienciaNotaFiscalAppService;
        }


        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:ciencia:pendente", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        [ProducesResponseType(typeof(IEnumerable<ProcessoAssinaturaDocumentoModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<SolicitacaoCienciaModel>> Post(
            SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel,
            CancellationToken cancellationToken = default)
        {
            var usuarioLogado = this.ObterUsuario().ActiveDirectoryId;

           var regristroCienciaNf = await _solicitacaoCienciaNotaFiscalAppService.RegistroCienciaPorUsuario(solicitacaoCienciaNfModel, usuarioLogado, cancellationToken);

            if (regristroCienciaNf.IsFailure)
                return this.Failure(regristroCienciaNf.Failure);

            return this.Success(regristroCienciaNf.Success);
        }
    }
}
