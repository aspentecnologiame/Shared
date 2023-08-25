using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System.Collections.Generic;
using System;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/consulta/pdf")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class PdfController : ControllerBase
    {

        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;

        public PdfController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [HttpGet("")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:verDetalhes")]
        public async Task<ActionResult<SaidaMaterialArquivoModel>> Get(
        int idSaidaMaterial,
        CancellationToken cancellationToken = default
            )
        {

           var saidaMaterial = await _saidaMaterialNotaFiscalAppService.ObterBase64DoPdf(idSaidaMaterial, cancellationToken);

            if (saidaMaterial.IsFailure)
                return this.Failure(saidaMaterial.Failure);

            
            return this.Success(saidaMaterial.Success);
        }
    }
}
