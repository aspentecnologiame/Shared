using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class RegistroCienciaController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;
        public RegistroCienciaController(IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AllowAnonymous]
        [HttpPost("")]
        [ProducesResponseType(typeof(DocumentoFI1548CienciaModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<DocumentoFI1548CienciaModel>> Post(
            DocumentoFI1548CienciaModel documentoFI1548CienciaModel,
            CancellationToken cancellationToken = default
        )
        {
            var regristroCiencia = await _documentoFI1548AppService.RegistroCienciaPorUsuario(documentoFI1548CienciaModel, this.ObterUsuario(), cancellationToken);

            if (regristroCiencia.IsFailure)
                return this.Failure(regristroCiencia.Failure);

            return this.Success(regristroCiencia.Success);
        }
    }
}
