using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
    public class EnviarParaAssinatura : ControllerBase
    {
        private readonly ILogger<EnviarParaAssinatura> _logger;
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;

        public EnviarParaAssinatura(ILogger<EnviarParaAssinatura> logger, ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService)
        {
            _logger = logger;
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:enviar:assinatura", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<int>> Post(SolicitacaoSaidaMaterialModel documento, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _solicitacaoSaidaMaterialAppService.EnviarParaAssinatura(documento, cancellationToken);

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