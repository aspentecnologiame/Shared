using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
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
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class AcoesSaidaMaterialNotaFiscalController : ControllerBase
    {
        private readonly ILogger<AcoesSaidaMaterialNotaFiscalController> _logger;
        private readonly ISaidaMaterialNotaFiscalAcaoAppService _saidaMaterialNotaFiscalAcaoAppService;

        public AcoesSaidaMaterialNotaFiscalController(ISaidaMaterialNotaFiscalAcaoAppService saidaMaterialNotaFiscalAcaoAppService, ILogger<AcoesSaidaMaterialNotaFiscalController> logger)
        {
            _saidaMaterialNotaFiscalAcaoAppService = saidaMaterialNotaFiscalAcaoAppService;
            _logger = logger;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:saida", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<SaidaMaterialNotaFiscalTipoAcaoModel>>> Get()
        {
            try
            {
                var response = await _saidaMaterialNotaFiscalAcaoAppService.ListarAcaoTipo();

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

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:saida,SaidaMaterialNF:acao:retorno,SaidaMaterialNF:acao:prorrogacao,SaidaMaterialNF:acao:baixasemretorno", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<int>> Post(SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var usuario = this.ObterUsuario();

                var response = await _saidaMaterialNotaFiscalAcaoAppService.InserirAcao(usuario.ActiveDirectoryId, saidaMaterialNotaFiscalAcaoModel, cancellationToken);

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
