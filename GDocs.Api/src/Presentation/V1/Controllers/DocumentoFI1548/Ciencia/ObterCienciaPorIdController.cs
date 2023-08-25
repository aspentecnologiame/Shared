using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Ciencia
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI1548/Ciencia/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterCienciaPorIdController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;
        public ObterCienciaPorIdController(IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [HttpGet("")]
        [ProducesResponseType(typeof(DocumentoFI1548CienciaModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<DocumentoFI1548CienciaModel>> Get(
            [FromQuery] int idSolicitacaoCiencia,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _documentoFI1548AppService.ObterCienciaPorId(idSolicitacaoCiencia, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);

        }
    }
}
