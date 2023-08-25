using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.ReportServerTools
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/ReportServerTools/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterSaidaMaterialNotaFiscalController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;
        public ObterSaidaMaterialNotaFiscalController(
            IGDocsCacheExternalService gDocsCacheExternalService, ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService)
        {
            _gDocsCacheExternalService = gDocsCacheExternalService;
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }

        [ApiExplorerSettings(GroupName = "Report Server Tools")]
        [HttpGet("{chave}.{format?}")]
        [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<SaidaMaterialNotaFiscalModel>>> Get(
             Guid chave,
             CancellationToken cancellationToken = default
         )
        {
            var obterFiltroSolicitacoesSaidaMaterial = await _gDocsCacheExternalService.ObterFiltrosSaidaMaterialNotaFiscalParaReportServer(chave);

            if (obterFiltroSolicitacoesSaidaMaterial.IsFailure)
                return this.Failure(obterFiltroSolicitacoesSaidaMaterial.Failure);

            var filtro = obterFiltroSolicitacoesSaidaMaterial.Success;

            var solicitacoesSaidaMaterial = await _saidaMaterialNotaFiscalAppService.PesquisarSaidaMateriaisNotaFiscalPorFiltro(filtro, cancellationToken);
            if (solicitacoesSaidaMaterial.IsFailure)
                return this.Failure(solicitacoesSaidaMaterial.Failure);

            FormatarCamposParaReport(solicitacoesSaidaMaterial);

            return this.Success(solicitacoesSaidaMaterial.Success);
        }

        private void FormatarCamposParaReport(TryException<IEnumerable<SaidaMaterialNotaFiscalModel>> response)
        {
            foreach (var item in response.Success)
            {
                string dataRetorno = item.Retorno.HasValue ? item.Retorno.Value.ToString("dd/MM/yyyy") : "-";
                item.DataRetornoFormatada = dataRetorno;
                item.DataCriacaoFormatada = item.DataCriacao.ToString("dd/MM/yyyy");

                var dataSaida = item.DataAcao == null ? "-" : item.DataAcao.Value.ToString("dd/MM/yyyy");
                item.DataSaidaFormatada = dataSaida;
            }
        }
    }
}
