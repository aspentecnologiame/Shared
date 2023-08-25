using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
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
    public class ObterSolicitacoesSaidaMaterialController : ControllerBase
    {
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;

        public ObterSolicitacoesSaidaMaterialController(
            ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService,
            IGDocsCacheExternalService gDocsCacheExternalService
            )
        {
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
            _gDocsCacheExternalService = gDocsCacheExternalService;
        }

        [ApiExplorerSettings(GroupName = "Report Server Tools")]
        [HttpGet("{chave}.{format?}")]
        [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<SolicitacaoSaidaMaterialModel>>> Get(
             Guid chave,
             CancellationToken cancellationToken = default
         )
        {
            var obterFiltroSolicitacoesSaidaMaterial = await _gDocsCacheExternalService.ObterFiltrosSolicitacoesSaidaMaterialParaReportServer(chave);

            if (obterFiltroSolicitacoesSaidaMaterial.IsFailure)
                return this.Failure(obterFiltroSolicitacoesSaidaMaterial.Failure);

            var filtro = obterFiltroSolicitacoesSaidaMaterial.Success;

            var solicitacoesSaidaMaterial = await _solicitacaoSaidaMaterialAppService.ConsultarSolicitacoesDeSaidaMateriaisPorFiltro(filtro, cancellationToken);
            if (solicitacoesSaidaMaterial.IsFailure)
                return this.Failure(solicitacoesSaidaMaterial.Failure);

            FormatarCamposParaReport(solicitacoesSaidaMaterial);

            return this.Success(solicitacoesSaidaMaterial.Success);
        }

        private void FormatarCamposParaReport(TryException<IEnumerable<SolicitacaoSaidaMaterialModel>> response)
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
