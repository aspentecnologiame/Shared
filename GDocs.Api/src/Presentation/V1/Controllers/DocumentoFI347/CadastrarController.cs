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
    public class CadastrarController : ControllerBase
    {
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;
        private readonly ISequencialService _sequencialService;
        private readonly ILogger<CadastrarController> _logger;

        public CadastrarController(ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService,
                                   ISequencialService sequencialService, ILogger<CadastrarController> logger)
        {
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
            _sequencialService = sequencialService;
            _logger = logger;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<IActionResult> Post(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!solicitacaoSaidaMaterialModel.PodeEditar())
                {

                    var sequencial = await _sequencialService.ObterProximoSequencialSaidaMaterias();
                    if (sequencial.IsFailure)
                        return this.Failure(sequencial.Failure);

                    solicitacaoSaidaMaterialModel.DefinirNumeroMaterial(sequencial.Success);

                }
                
                solicitacaoSaidaMaterialModel.DefinirAutorEResponsavel(this.ObterUsuario().ActiveDirectoryId);

                var response = await _solicitacaoSaidaMaterialAppService.Inserir(solicitacaoSaidaMaterialModel, cancellationToken);

                if (response.IsFailure)
                    return this.Failure(response.Failure);

                return Ok(response.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

    }
}
