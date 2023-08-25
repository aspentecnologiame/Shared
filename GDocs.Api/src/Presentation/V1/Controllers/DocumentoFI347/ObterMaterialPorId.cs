using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SolicitacaoSaidaMaterial
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterMaterialPorId : ControllerBase
    {
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;
        private readonly ILogger<CadastrarController> _logger;

        public ObterMaterialPorId(ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService,
                                   ISequencialService sequencialService, ILogger<CadastrarController> logger)
        {
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
            _logger = logger;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<IActionResult> Get(int ssmId, CancellationToken cancellationToken = default)
        {
            try
            {
                var material = await _solicitacaoSaidaMaterialAppService.ObterMaterialComItensPorId(ssmId, cancellationToken);

                if (material.IsFailure)
                    return this.Failure(material.Failure);

                return Ok(material.Success);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
