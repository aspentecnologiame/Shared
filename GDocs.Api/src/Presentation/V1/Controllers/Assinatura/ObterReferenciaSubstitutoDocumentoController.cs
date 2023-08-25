using ICE.GDocs.Api.Security;
using ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.Assinatura
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Assinatura/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterReferenciaSubstitutoDocumentoController : ControllerBase
    {

        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;
        private readonly ILogger<ConsultaFiltrarController> _logger;

        public ObterReferenciaSubstitutoDocumentoController(
            IDocumentoFI1548AppService documentoFI1548AppService,
            ILogger<ConsultaFiltrarController> logger
            )
        {
            _documentoFI1548AppService = documentoFI1548AppService;
            _logger = logger;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<AssinaturaInformacoesModel>> Post(
            AssinaturaInformacoesFilterModel filtro,
            CancellationToken cancellationToken = default
        )
        {
            try
            {

                var response = await _documentoFI1548AppService.ObterReferenciaSubstitutoOrigemDocumento(filtro.NumeroDocumento, cancellationToken);

                if (response.IsFailure)
                    return this.Failure(response.Failure);

                var infoList = response.Success;

                if (infoList == null)
                    return NotFound();

                return this.Success(infoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}