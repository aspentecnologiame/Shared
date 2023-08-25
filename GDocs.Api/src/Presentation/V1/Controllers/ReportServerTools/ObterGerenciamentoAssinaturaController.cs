using ICE.GDocs.Application.GDocs;
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
    public class ObterGerenciamentoAssinaturaController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;

        public ObterGerenciamentoAssinaturaController(
            IAssinaturaAppService assinaturaAppService,
            IGDocsCacheExternalService gDocsCacheExternalService
            )
        {
            _assinaturaAppService = assinaturaAppService;
            _gDocsCacheExternalService = gDocsCacheExternalService;
        }

        [ApiExplorerSettings(GroupName = "Report Server Tools")]
        [HttpGet("{chave}.{format?}")]
        [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<AssinaturaInformacoesModel>>> Get(
            Guid chave,
            CancellationToken cancellationToken = default
        )
        {
            var obterFiltroGerenciamentoAssinaturaParaReportServer = await _gDocsCacheExternalService.ObterFiltrosGerenciamentoAssinaturaParaReportServer(chave);

            if (obterFiltroGerenciamentoAssinaturaParaReportServer.IsFailure)
                return this.Failure(obterFiltroGerenciamentoAssinaturaParaReportServer.Failure);

            var filtro = obterFiltroGerenciamentoAssinaturaParaReportServer.Success;

            var response = await _assinaturaAppService.ObterInformacoesPorNumeroNomeStatusAutorPeriodo(filtro, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            FormatarDataCriacaoParaReport(response);

            return this.Success(response.Success);
        }

        private void FormatarDataCriacaoParaReport(TryException<IEnumerable<AssinaturaInformacoesModel>> response)
        {
            foreach (var item in response.Success)
            {
                item.DataCriacaoFormatada = item.DataCriacao.ToString("dd/MM/yyyy");
            }
        }
    }
}
