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
    public class ObterDadosStatusPagamento : Controller
    {
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ObterDadosStatusPagamento(
            IGDocsCacheExternalService gDocsCacheExternalService,
            IDocumentoFI1548AppService documentoFI1548AppService
        )
        {
            _gDocsCacheExternalService = gDocsCacheExternalService;
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Report Server Tools")]
        [HttpGet("{chave}.{format?}")]
        [Produces("application/xml")]
        public async Task<ActionResult<IEnumerable<DocumentoFI1548Model>>> Get(
            Guid chave,
            CancellationToken cancellationToken = default
        )
        {
            var obterFiltrosDocumentoFI1548ParaReportServer = await _gDocsCacheExternalService.ObterFiltrosDocumentoFI1548ParaReportServer(chave);
            if (obterFiltrosDocumentoFI1548ParaReportServer.IsFailure)
                return this.Failure(obterFiltrosDocumentoFI1548ParaReportServer.Failure);

            var filtro = obterFiltrosDocumentoFI1548ParaReportServer.Success;

            var response = await _documentoFI1548AppService.ObterPorNumeroTipoPagamentoAutorPeriodo(filtro, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);
        }
    }
}
