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
    public class GerenciamentoController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;
        private readonly IAuthorizationService _authService;
        private readonly ILogger<ConsultaFiltrarController> _logger;

        public GerenciamentoController(
            IAssinaturaAppService assinaturaAppService,
            IAuthorizationService authService,
            ILogger<ConsultaFiltrarController> logger
            )
        {
            _assinaturaAppService = assinaturaAppService;
            _authService = authService;
            _logger = logger;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("filtrar")]
        public async Task<ActionResult<AssinaturaInformacoesModel>> Post(
            AssinaturaInformacoesFilterModel filtro,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                filtro.UsuariosGuidAd = await this.HasRoleAsync(_authService, "assinatura:gerenciardocumentos:todosusuarios")
                    ? filtro.UsuariosGuidAd : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };

                var response = await _assinaturaAppService.ObterInformacoesPorNumeroNomeStatusAutorPeriodo(filtro, cancellationToken);

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