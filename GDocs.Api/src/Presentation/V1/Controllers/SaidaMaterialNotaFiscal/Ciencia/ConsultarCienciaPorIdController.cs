using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Ciencia
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/ciencia/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultarCienciaPorIdController : ControllerBase
    {

        private readonly ISolicitacaoCienciaNotaFiscalAppService _solicitacaoCienciaNotaFiscalAppService;
        private readonly ISaidaMaterialNotaFiscalAcaoAppService   _saidaMaterialNotaFiscalAcaoAppService;


        public ConsultarCienciaPorIdController(
            ISolicitacaoCienciaNotaFiscalAppService solicitacaoCienciaNotaFiscalAppService, 
            ISaidaMaterialNotaFiscalAcaoAppService saidaMaterialNotaFiscalAcaoAppService)
        {
            _solicitacaoCienciaNotaFiscalAppService = solicitacaoCienciaNotaFiscalAppService;
            _saidaMaterialNotaFiscalAcaoAppService = saidaMaterialNotaFiscalAcaoAppService;
        }


        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:ciencia:pendente", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<SaidaMaterialNotaFiscalCienciaModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<SaidaMaterialNotaFiscalCienciaModel>>> Get(
            int idCienciaNotaFiscal,
           CancellationToken cancellationToken = default)
        {

            var listaCiencia = await _solicitacaoCienciaNotaFiscalAppService.ObterCienciaNfPorId(idCienciaNotaFiscal, cancellationToken);

            if (listaCiencia.IsFailure)
                return this.Failure(listaCiencia.Failure);

            var ciencia = listaCiencia.Success;

            var historico = await _saidaMaterialNotaFiscalAcaoAppService.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(
                 ciencia.IdSaidaMaterialNotaFiscal,
                 SaidaMaterialNotaFiscalTipoAcao.SolicitacaoProrrogacao,
                cancellationToken);

            if (historico.IsFailure)
                return this.Failure(historico.Failure);

            var cienciaUsuarioAprovar = await _solicitacaoCienciaNotaFiscalAppService.ListarUsuarioAprovacao(idCienciaNotaFiscal, cancellationToken);

            if (cienciaUsuarioAprovar.IsFailure)
                return this.Failure(listaCiencia.Failure);


            return this.Success(new CienciaEhHistoricoResponseModel()
           {
               SolicitacaoCiencia = ciencia,
               HistoricoProrrogacoes = historico.Success,
               CienciaUsuarioAprovacao = cienciaUsuarioAprovar.Success
           });

        }
    }
}
