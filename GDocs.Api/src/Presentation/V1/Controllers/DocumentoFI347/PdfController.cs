using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI347
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class PdfController : ControllerBase
    {        
        private readonly ILogService _logService;
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;        

        public PdfController(            
            ILogService logService,
            ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService
            )
        {            
            _logService = logService;
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;            
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [HttpGet("")]
        [AuthorizeBearer(Roles = "fi347:consultar")]
        public async Task<ActionResult<DocumentoPdfModel>> Get(
            int idSaidaMaterial,
            CancellationToken cancellationToken = default
        )
        {
            await _logService.AdicionarRastreabilidade(new { Id = idSaidaMaterial }, "Iniciando consulta do pdf de saida de material.");

            var saidaMaterial = await _solicitacaoSaidaMaterialAppService.ObterBase64DoPdf(idSaidaMaterial, cancellationToken);

            if (saidaMaterial.IsFailure)
                return this.Failure(saidaMaterial.Failure);

            await _logService.AdicionarRastreabilidade(new { Id = idSaidaMaterial }, "base64 pdf de saida de material retornado.");

            return this.Success(new DocumentoPdfModel() { PdfDocumentBase64 = saidaMaterial.Success });
        }
    }
}
