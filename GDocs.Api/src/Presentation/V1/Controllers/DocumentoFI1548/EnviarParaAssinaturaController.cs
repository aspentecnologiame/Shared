using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
    public class EnviarParaAssinatura : ControllerBase
    {
        private readonly ILogger<EnviarParaAssinatura> _logger;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public EnviarParaAssinatura(
            ILogger<EnviarParaAssinatura> logger,
            IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _logger = logger;
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<int>> Post(
            DocumentoEhAssinaturaPassosModel documentoEhAssinaturaPasso,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var response = await _documentoFI1548AppService.EnviarParaAssinatura(documentoEhAssinaturaPasso, cancellationToken);

                if (response.IsFailure)
                    return this.Failure(response.Failure);

                return this.Success(response.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
