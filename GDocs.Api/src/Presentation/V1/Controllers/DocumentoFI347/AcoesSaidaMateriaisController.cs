using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    public class AcoesSaidaMateriaisController : ControllerBase
    {
        private readonly ILogger<AcoesSaidaMateriaisController> _logger;
        private readonly ISolicitacaoSaidaMaterialAcaoAppService _solicitacaoSaidaMaterialAcaoAppService;

        public AcoesSaidaMateriaisController(ISolicitacaoSaidaMaterialAcaoAppService solicitacaoSaidaMaterialAcaoAppService, ILogger<AcoesSaidaMateriaisController> logger)
        {
            _solicitacaoSaidaMaterialAcaoAppService = solicitacaoSaidaMaterialAcaoAppService;
            _logger = logger;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:acoes:saida", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<SolicitacaoSaidaMaterialAcaoTipoModel>>> Get()
        {
            try
            {
                var response = await _solicitacaoSaidaMaterialAcaoAppService.ListarAcaoTipo();

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

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:acoes:saida,fi347:acoes:retorno,fi347:acoes:prorrogacao,fi347:acoes:baixasemretorno", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<int>> Post(SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var usuario = this.ObterUsuario();

                var response = await _solicitacaoSaidaMaterialAcaoAppService.InserirAcao(usuario.ActiveDirectoryId, solicitacaoSaidaMaterialAcaoModel, cancellationToken);

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
