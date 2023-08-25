using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI1548/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class PdfController : ControllerBase
    {        
        private readonly ILogService _logService;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;        

        public PdfController(            
            ILogService logService,
            IDocumentoFI1548AppService documentoFI1548AppService                    
            )
        {            
            _logService = logService;
            _documentoFI1548AppService = documentoFI1548AppService;            
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [HttpGet("")]
        [AuthorizeBearer(Roles = "fi1548:consultar")]
        public async Task<ActionResult<DocumentoPdfModel>> Get(
            int idDocumento,
            CancellationToken cancellationToken = default
        )
        {
            await _logService.AdicionarRastreabilidade(new { Id = idDocumento }, "Iniciando consulta do pdf.");

            var documento = await _documentoFI1548AppService.ObterBase64DoPdf(idDocumento, cancellationToken);

            if (documento.IsFailure)
                return this.Failure(documento.Failure);

            await _logService.AdicionarRastreabilidade(new { Id = idDocumento }, "base64 pdf retornado.");

            return this.Success(new DocumentoPdfModel() { PdfDocumentBase64 = documento.Success });
        }
    }
}
